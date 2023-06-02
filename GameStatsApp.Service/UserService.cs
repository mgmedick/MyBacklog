using System;
using GameStatsApp.Interfaces.Repositories;
using GameStatsApp.Interfaces.Services;
using GameStatsApp.Model;
using GameStatsApp.Model.Data;
using GameStatsApp.Model.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;
using GameStatsApp.Common.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using System.Security.Claims;

namespace GameStatsApp.Service
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepo = null;
        private readonly IEmailService _emailService = null;
        private readonly IHttpContextAccessor _context = null;
        private readonly IConfiguration _config = null;

        public UserService(IUserRepository userRepo, IEmailService emailService, IHttpContextAccessor context, IConfiguration config)
        {
            _userRepo = userRepo;
            _emailService = emailService;
            _context = context;
            _config = config;
        }

        public async Task SendActivationEmail(string email)
        {
            var hashKey = _config.GetSection("SiteSettings").GetSection("HashKey").Value;
            var baseUrl = string.Format("{0}://{1}{2}", _context.HttpContext.Request.Scheme, _context.HttpContext.Request.Host, _context.HttpContext.Request.PathBase);
            var queryParams = string.Format("email={0}&expirationTime={1}", email, DateTime.UtcNow.AddHours(48).Ticks);
            var token = queryParams.GetHMACSHA256Hash(hashKey);

            var activateUser = new
            {
                Email = email,
                ActivateLink = string.Format("{0}/Home/Activate?{1}&token={2}", baseUrl, queryParams, token)
            };

            await _emailService.SendEmailTemplate(email, "Create your gamestatsapp.com account", Template.ActivateEmail.ToString(), activateUser);
        }

        public ActivateViewModel GetActivateUser(string email, long expirationTime, string token)
        {
            var hashKey = _config.GetSection("SiteSettings").GetSection("HashKey").Value;
            var strToHash = string.Format("email={0}&expirationTime={1}", email, expirationTime);
            var hash = strToHash.GetHMACSHA256Hash(hashKey);
            var expirationDate = new DateTime(expirationTime);
            var emailExists = _userRepo.GetUsers(i => i.Email == email).Any();
            var isValid = (hash == token) && expirationDate > DateTime.UtcNow && !emailExists;
            var activateUserVM = new ActivateViewModel() { Email = email, IsValid = isValid };

            return activateUserVM;
        }

        public async Task SendConfirmRegistrationEmail(string email, string username)
        {
            var confirmRegistration = new
            {
                Username = username,
                SupportEmail = _config.GetSection("SiteSettings").GetSection("FromEmail").Value
            };

            await _emailService.SendEmailTemplate(email, "Thanks for registering at gamestatsapp.com", Template.ConfirmRegistration.ToString(), confirmRegistration);
        }

        public async Task SendResetPasswordEmail(string email)
        {
            var user = _userRepo.GetUsers(i => i.Email == email).FirstOrDefault();
            var hashKey = _config.GetSection("SiteSettings").GetSection("HashKey").Value;
            var baseUrl = string.Format("{0}://{1}{2}", _context.HttpContext.Request.Scheme, _context.HttpContext.Request.Host, _context.HttpContext.Request.PathBase);
            var queryParams = string.Format("username={0}&email={1}&expirationTime={2}", user.Username, user.Email, DateTime.UtcNow.AddHours(48).Ticks);
            var token = string.Format("{0}&password={1}", queryParams, user.Password).GetHMACSHA256Hash(hashKey);

            var passwordReset = new
            {
                Username = user.Username,
                ResetPassLink = string.Format("{0}/Home/ChangePassword?{1}&token={2}", baseUrl, queryParams, token)
            };

            await _emailService.SendEmailTemplate(user.Email, "Reset your gamestatsapp.com password", Template.ResetPasswordEmail.ToString(), passwordReset);
        }
        
        public ChangePasswordViewModel GetChangePassword(string email, long expirationTime, string token)
        {
            var user = _userRepo.GetUsers(i => i.Email == email).FirstOrDefault();
            var hashKey = _config.GetSection("SiteSettings").GetSection("HashKey").Value;
            var strToHash = string.Format("email={0}&expirationTime={1}&password={2}", email, expirationTime, user.Password);
            var hash = strToHash.GetHMACSHA256Hash(hashKey);
            var expirationDate = new DateTime(expirationTime);
            var isValid = (hash == token) && expirationDate > DateTime.UtcNow;
            var changePassVM = new ChangePasswordViewModel() { IsValid = isValid };

            return changePassVM;
        }

        public IEnumerable<User> GetUsers(Expression<Func<User, bool>> predicate)
        {
            return _userRepo.GetUsers(predicate);
        }

        public IEnumerable<UserView> GetUserViews(Expression<Func<UserView, bool>> predicate)
        {
            return _userRepo.GetUserViews(predicate);
        }

        public void CreateUser(string email, string username, string pass)
        {
            var isdarktheme = (_context.HttpContext.Request.Cookies["theme"] ?? _config.GetSection("SiteSettings").GetSection("DefaultTheme").Value) == "theme-dark";

            var user = new User()
            {
                Username = username,
                Password = pass.HashString(),
                Email = email,
                Active = true,
                CreatedBy = 1,
                CreatedDate = DateTime.UtcNow
            };

            _userRepo.SaveUser(user);

            var userSetting = new UserSetting() {
                UserID = user.ID,
                IsDarkTheme = isdarktheme
            };

            _userRepo.SaveUserSetting(userSetting);
        }

        public void ChangeUserPassword(string email, string pass)
        {
            var user = _userRepo.GetUsers(i => i.Email == email).FirstOrDefault();
            user.Password = pass.HashString();
            user.ModifiedBy = user.ID;
            user.ModifiedDate = DateTime.UtcNow;

            _userRepo.SaveUser(user);
        }

        public UserViewModel GetUser(int userID)
        {
            var userView = _userRepo.GetUserViews(i => i.UserID == userID).FirstOrDefault();
            var userVM = new UserViewModel(userView);

            return userVM;
        }

        public void SaveUser(UserViewModel userVM, int currUserID)
        {
            var user = _userRepo.GetUsers(i => i.ID == userVM.UserID).FirstOrDefault();

            if (user != null)
            {
                var userSetting = new UserSetting()
                {
                    UserID = userVM.UserID,
                    IsDarkTheme = userVM.IsDarkTheme
                };

                _userRepo.SaveUserSetting(userSetting);

                user.ModifiedDate = DateTime.UtcNow;
                user.ModifiedBy = currUserID;
                _userRepo.SaveUser(user);
            }
        }

        public void UpdateIsDarkTheme(int currUserID, bool isDarkTheme)
        {
            var user = _userRepo.GetUsers(i => i.ID == currUserID).FirstOrDefault();

            if (user != null)
            {
                var userSetting = new UserSetting()
                {
                    UserID = user.ID,
                    IsDarkTheme = isDarkTheme
                };

                _userRepo.SaveUserSetting(userSetting);
            }
        }

        //jqvalidate
        public bool EmailExists(string email)
        {
            var result = _userRepo.GetUsers(i => i.Email == email & i.Active).Any();

            return result;
        }

        public bool PasswordMatches(string password, string email)
        {
            var result = false;
            var user = _userRepo.GetUsers(i => i.Email == email && i.Active).FirstOrDefault();

            if (user != null)
            {
                result = password.VerifyHash(user.Password);
            }

            return result;
        }

        public bool UsernameExists(string username, bool activeFilter)
        {
            var result = _userRepo.GetUsers(i => i.Username == username && (i.Active || i.Active == activeFilter)).Any();

            return result;
        }

        public bool EmailExists(string email, bool activeFilter)
        {
            var result = _userRepo.GetUsers(i => i.Email == email && (i.Active || i.Active == activeFilter)).Any();

            return result;
        }        

        public IEnumerable<SearchResult> SearchUsers(string searchText)
        {
            return _userRepo.SearchUsers(searchText);
        }        
    }
}
