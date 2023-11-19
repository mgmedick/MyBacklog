using System;
using GameStatsApp.Interfaces.Repositories;
using GameStatsApp.Interfaces.Services;
using GameStatsApp.Model;
using GameStatsApp.Model.Data;
using GameStatsApp.Model.JSON;
using GameStatsApp.Model.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Expressions;
using GameStatsApp.Common.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace GameStatsApp.Service
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepo = null;
        private readonly IUserListRepository _userListRepo = null;
        private readonly IEmailService _emailService = null;
        private readonly IAuthService _authService = null;
        private readonly IHttpContextAccessor _context = null;
        private readonly IConfiguration _config = null;

        public UserService(IUserRepository userRepo, IUserListRepository userListRepo, IEmailService emailService, IAuthService authService, IHttpContextAccessor context, IConfiguration config)
        {
            _userRepo = userRepo;
            _userListRepo = userListRepo;
            _emailService = emailService;
            _authService = authService;
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
                ActivateLink = string.Format("{0}/Activate?{1}&token={2}", baseUrl, queryParams, token)
            };

            await _emailService.SendEmailTemplate(email, "Create your mybacklog.io account", Template.ActivateEmail.ToString(), activateUser);
        }

        public ActivateViewModel GetActivateUser(string email, long expirationTime, string token)
        {
            var hashKey = _config.GetSection("SiteSettings").GetSection("HashKey").Value;
            var strToHash = string.Format("email={0}&expirationTime={1}", email, expirationTime);
            var hash = strToHash.GetHMACSHA256Hash(hashKey);
            var expireDate = new DateTime(expirationTime);
            var emailExists = _userRepo.GetUsers(i => i.Email == email).Any();
            var isValid = (hash == token) && expireDate > DateTime.UtcNow && !emailExists;
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

            await _emailService.SendEmailTemplate(email, "Thanks for registering at mybacklog.io", Template.ConfirmRegistration.ToString(), confirmRegistration);
        }

        public async Task SendResetPasswordEmail(string email)
        {
            var user = _userRepo.GetUsers(i => i.Email == email).FirstOrDefault();
            var hashKey = _config.GetSection("SiteSettings").GetSection("HashKey").Value;
            var baseUrl = string.Format("{0}://{1}{2}", _context.HttpContext.Request.Scheme, _context.HttpContext.Request.Host, _context.HttpContext.Request.PathBase);
            var queryParams = string.Format("email={0}&expirationTime={1}", user.Email, DateTime.UtcNow.AddHours(48).Ticks);
            var token = string.Format("{0}&password={1}", queryParams, user.Password).GetHMACSHA256Hash(hashKey);

            var passwordReset = new
            {
                Username = user.Username,
                ResetPassLink = string.Format("{0}/Home/ChangePassword?{1}&token={2}", baseUrl, queryParams, token)
            };

            await _emailService.SendEmailTemplate(user.Email, "Reset your mybacklog.io password", Template.ResetPasswordEmail.ToString(), passwordReset);
        }
        
        public ChangePasswordViewModel GetChangePassword(string email, long expirationTime, string token)
        {
            var changePassVM = new ChangePasswordViewModel();
            
            var user = _userRepo.GetUsers(i => i.Email == email).FirstOrDefault();
            if (user != null)
            {
                var hashKey = _config.GetSection("SiteSettings").GetSection("HashKey").Value;
                var strToHash = string.Format("email={0}&expirationTime={1}&password={2}", email, expirationTime, user.Password);
                var hash = strToHash.GetHMACSHA256Hash(hashKey);
                var expireDate = new DateTime(expirationTime);
                var isValid = (hash == token) && expireDate > DateTime.UtcNow;
                changePassVM.Email = user.Email;
                changePassVM.EmailToken = user.Email.GetHMACSHA256Hash(hashKey);
                changePassVM.IsValid = isValid;
            }

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

        public int CreateDemoUser()
        {
            var emailDomain = _context.HttpContext.Request.Host;
            var username = "demo" + StringExtensions.GeneratePassword(10, 0);
            var email = string.Format("{0}@{1}", username, emailDomain);
            var password = StringExtensions.GeneratePassword(15, 2);

            var userID = CreateUser(email, username, password);

            return userID;     
        }

        public int CreateUser(string email, string username, string pass)
        {
            var user = new User()
            {
                Email = email,
                Username = username,
                Password = pass.HashString(),
                Active = true,
                CreatedBy = 1,
                CreatedDate = DateTime.UtcNow
            };

            _userRepo.SaveUser(user);

            var userSetting = new UserSetting() {
                UserID = user.ID,
                IsDarkTheme = false
            };

            _userRepo.SaveUserSetting(userSetting);      
 
            var userLists = _userListRepo.GetDefaultLists().Select(i => new UserList() { UserID = user.ID,
                                                                   Name = i.Name, 
                                                                   DefaultListID = i.ID,
                                                                   Active = true,
                                                                   CreatedDate = DateTime.UtcNow }).ToList();

            _userListRepo.SaveUserLists(userLists);    

            return user.ID;  
        }

        public void ChangeUserPassword(string email, string pass)
        {
            var user = _userRepo.GetUsers(i => i.Email == email).FirstOrDefault();
            if (user != null)
            {
                user.Password = pass.HashString();
                user.ModifiedBy = user.ID;
                user.ModifiedDate = DateTime.UtcNow;

                _userRepo.SaveUser(user);
            }
        }

        public void ChangeUsername(int userID, string username)
        {
            var user = _userRepo.GetUsers(i => i.ID == userID).FirstOrDefault();
            if (user != null)
            {
                user.Username = username; 
                user.ModifiedBy = user.ID;
                user.ModifiedDate = DateTime.UtcNow;

                _userRepo.SaveUser(user);
            }            
        }        
          
        public void DeleteUser(int userID)
        {
            var user = _userRepo.GetUsers(i => i.ID == userID).FirstOrDefault();
            if (user != null)
            {
                user.Active = false; 
                user.Deleted = true; 
                user.ModifiedBy = user.ID;
                user.ModifiedDate = DateTime.UtcNow;

                _userRepo.SaveUser(user);
            }
        }

        public void SaveUserAccount(int userID, int accountTypeID, TokenResponse tokenResponse)
        {
            var userAccount = _userRepo.GetUserAccounts(i => i.UserID == userID && i.AccountTypeID == accountTypeID).FirstOrDefault();
            var userAccountTokens = new List<UserAccountToken>();
            UserList userList = null;

            if (userAccount == null)
            {
                userAccount = new UserAccount()
                {
                    UserID = userID,
                    AccountTypeID = accountTypeID,
                    AccountUserID = tokenResponse.AccountUserID,
                    AccountUserHash = tokenResponse.AccountUserHash,                   
                    CreatedDate = DateTime.UtcNow
                };

                userList =  new UserList()
                { 
                    UserID = userID,
                    Name = ((AccountType)userAccount.AccountTypeID).ToString(),
                    Active = true,
                    CreatedDate = DateTime.UtcNow
                };
            }
            else
            {
                userAccount.AccountUserID = tokenResponse.AccountUserID;
                userAccount.AccountUserHash = tokenResponse.AccountUserHash;
                userAccount.ModifiedDate = DateTime.UtcNow;
            }

            if (!string.IsNullOrWhiteSpace(tokenResponse.Token))
            {
                userAccountTokens = new List<UserAccountToken>() {
                    new UserAccountToken() {
                            TokenTypeID = (int)TokenType.Access,
                            Token = tokenResponse.Token,
                            IssuedDate = tokenResponse.IssuedDate,
                            ExpireDate = tokenResponse.ExpireDate
                        } 
                };

                if (!string.IsNullOrWhiteSpace(tokenResponse.RefreshToken))
                {
                    userAccountTokens.Add(new UserAccountToken() {
                        TokenTypeID = (int)TokenType.Refresh,
                        Token = tokenResponse.RefreshToken
                    });
                }
            }

            _userRepo.SaveUserAccount(userAccount);
            
            if (userAccountTokens.Any())
            {
                _userRepo.SaveUserAccountTokens(userAccount.ID, userAccountTokens);
            }

            if (userList != null)
            {
                userList.UserAccountID = userAccount.ID;
                _userListRepo.SaveUserList(userList);
            }
        }
        
        public IEnumerable<UserAccountViewModel> GetUserAccounts(int userID)
        {
            var userAccountVMs = _userRepo.GetUserAccountViews(i => i.UserID == userID).Select(i => new UserAccountViewModel(i)).ToList();

            return userAccountVMs;
        }

        public async Task<UserAccountView> GetRefreshedUserAccount(int userID, int userAccountID)
        {
            var authUrl = string.Empty;
            var redirectUrl = string.Empty;
            var userAccountVW = _userRepo.GetUserAccountViews(i => i.ID == userAccountID).FirstOrDefault();
            
            if (userAccountVW.ExpireDate.HasValue && userAccountVW.ExpireDate < DateTime.UtcNow && !string.IsNullOrWhiteSpace(userAccountVW.RefreshToken))
            {
                TokenResponse tokenResponse = null;

                switch(userAccountVW.AccountTypeID)
                {
                    case (int)AccountType.Xbox:
                        tokenResponse = await _authService.ReAuthenticateMicrosoft(userAccountVW.RefreshToken);
                        break;
                }

                if (tokenResponse != null)
                {
                    SaveUserAccount(userID, userAccountVW.AccountTypeID, tokenResponse);
                    userAccountVW = _userRepo.GetUserAccountViews(i => i.ID == userAccountID).FirstOrDefault();  
                }
            }

            return userAccountVW;
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
            var hashKey = _config.GetSection("SiteSettings").GetSection("HashKey").Value;

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
    }
}
