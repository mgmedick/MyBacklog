using System;
using SpeedRunApp.Interfaces.Repositories;
using SpeedRunApp.Interfaces.Services;
using SpeedRunApp.Model;
using SpeedRunApp.Model.Data;
using SpeedRunApp.Model.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;
using SpeedRunCommon.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using System.Security.Claims;

namespace SpeedRunApp.Service
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepo = null;
        private readonly IEmailService _emailService = null;
        private readonly IHttpContextAccessor _context = null;
        private readonly IConfiguration _config = null;
        private readonly ISpeedRunRepository _speedRunRepo = null;

        public UserService(IUserRepository userRepo, IEmailService emailService, IHttpContextAccessor context, IConfiguration config)
        {
            _userRepo = userAcctRepo;
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

            var activateUserAcct = new
            {
                Email = email,
                ActivateLink = string.Format("{0}/Home/Activate?{1}&token={2}", baseUrl, queryParams, token)
            };

            await _emailService.SendEmailTemplate(email, "Create your gamestatsapp.com account", Template.ActivateEmail.ToString(), activateUserAcct);
        }

        public ActivateViewModel GetActivateUser(string email, long expirationTime, string token)
        {
            var hashKey = _config.GetSection("SiteSettings").GetSection("HashKey").Value;
            var strToHash = string.Format("email={0}&expirationTime={1}", email, expirationTime);
            var hash = strToHash.GetHMACSHA256Hash(hashKey);
            var expirationDate = new DateTime(expirationTime);
            var emailExists = _userRepo.GetUsers(i => i.Email == email).Any();
            var isValid = (hash == token) && expirationDate > DateTime.UtcNow && !emailExists;
            var activateUserAcctVM = new ActivateViewModel() { IsValid = isValid };

            return activateUserAcctVM;
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

        public async Task SendResetPasswordEmail(string username)
        {
            var userAcct = _userRepo.GetUsers(i => i.Username == username).FirstOrDefault();
            var hashKey = _config.GetSection("SiteSettings").GetSection("HashKey").Value;
            var baseUrl = string.Format("{0}://{1}{2}", _context.HttpContext.Request.Scheme, _context.HttpContext.Request.Host, _context.HttpContext.Request.PathBase);
            var queryParams = string.Format("username={0}&email={1}&expirationTime={2}", userAcct.Username, userAcct.Email, DateTime.UtcNow.AddHours(48).Ticks);
            var token = string.Format("{0}&password={1}", queryParams, userAcct.Password).GetHMACSHA256Hash(hashKey);

            var passwordReset = new
            {
                Username = userAcct.Username,
                ResetPassLink = string.Format("{0}/Home/ChangePassword?{1}&token={2}", baseUrl, queryParams, token)
            };

            await _emailService.SendEmailTemplate(userAcct.Email, "Reset your gamestatsapp.com password", Template.ResetPasswordEmail.ToString(), passwordReset);
        }
        
        public ChangePasswordViewModel GetChangePassword(string username, string email, long expirationTime, string token)
        {
            var userAcct = _userRepo.GetUsers(i => i.Username == username).FirstOrDefault();
            var hashKey = _config.GetSection("SiteSettings").GetSection("HashKey").Value;
            var strToHash = string.Format("username={0}&email={1}&expirationTime={2}&password={3}", username, email, expirationTime, userAcct.Password);
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

        public void CreateUser(string username, string pass)
        {
            var email = _context.HttpContext.Session.Get<string>("Email");
            var isdarktheme = (_context.HttpContext.Request.Cookies["theme"] ?? _config.GetSection("SiteSettings").GetSection("DefaultTheme").Value) == "theme-dark";

            var userAcct = new User()
            {
                Username = username,
                Password = pass.HashString(),
                Email = email,
                Active = true,
                CreatedBy = 1,
                CreatedDate = DateTime.UtcNow
            };

            _userRepo.SaveUser(userAcct);

            var userAcctSetting = new UserSetting() {
                UserID = userAcct.ID,
                IsDarkTheme = isdarktheme
            };

            _userRepo.SaveUserSetting(userAcctSetting);
        }

        public void ChangeUserPassword(string username, string pass)
        {
            var userAcct = _userRepo.GetUsers(i => i.Username == username).FirstOrDefault();
            userAcct.Password = pass.HashString();
            userAcct.ModifiedBy = userAcct.ID;
            userAcct.ModifiedDate = DateTime.UtcNow;

            _userRepo.SaveUser(userAcct);
        }

        public UserViewModel GetUser(int userID)
        {
            var userAcctView = _userRepo.GetUserViews(i => i.UserID == userID).FirstOrDefault();
            var userAcctVM = new UserViewModel(userAcctView);
            userAcctVM.SpeedRunListCategories = _speedRunRepo.SpeedRunListCategories().ToList();

            return userAcctVM;
        }

        public void SaveUser(UserViewModel userAcctVM, int currUserID)
        {
            var userAcct = _userRepo.GetUsers(i => i.ID == userAcctVM.UserID).FirstOrDefault();

            if (userAcct != null)
            {
                var userAcctSetting = new UserSetting()
                {
                    UserID = userAcctVM.UserID,
                    IsDarkTheme = userAcctVM.IsDarkTheme
                };

                _userRepo.SaveUserSetting(userAcctSetting);

                userAcct.ModifiedDate = DateTime.UtcNow;
                userAcct.ModifiedBy = currUserID;
                _userRepo.SaveUser(userAcct);
            }
        }

        public void UpdateIsDarkTheme(int currUserID, bool isDarkTheme)
        {
            var userAcct = _userRepo.GetUsers(i => i.ID == currUserID).FirstOrDefault();

            if (userAcct != null)
            {
                var userAcctSetting = new UserSetting()
                {
                    UserID = userAcct.ID,
                    IsDarkTheme = isDarkTheme
                };

                _userRepo.SaveUserSetting(userAcctSetting);
            }
        }

        //jqvalidate
        public bool EmailExists(string email)
        {
            var result = _userRepo.GetUsers(i => i.Email == email).Any();

            return result;
        }

        public bool PasswordMatches(string password, string username)
        {
            var result = false;
            var userAcct = _userRepo.GetUsers(i => i.Username == username && i.Active).FirstOrDefault();

            if (userAcct != null)
            {
                result = password.VerifyHash(userAcct.Password);
            }

            return result;
        }

        public bool UsernameExists(string username, bool activeFilter)
        {
            var result = _userRepo.GetUsers(i => i.Username == username && (i.Active || i.Active == activeFilter)).Any();

            return result;
        }
    }
}
