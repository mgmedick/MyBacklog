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
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;
using GameStatsApp.Common.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using System.Security.Claims;
using Newtonsoft.Json;

namespace GameStatsApp.Service
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepo = null;
        private readonly IEmailService _emailService = null;
        private readonly IAuthService _authService = null;
        private readonly IGameRepository _gameRepo = null;
        private readonly IHttpContextAccessor _context = null;
        private readonly IConfiguration _config = null;

        public UserService(IUserRepository userRepo, IEmailService emailService, IAuthService authService, IGameRepository gameRepo, IHttpContextAccessor context, IConfiguration config)
        {
            _userRepo = userRepo;
            _emailService = emailService;
            _authService = authService;
            _gameRepo = gameRepo;
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

            await _emailService.SendEmailTemplate(email, "Thanks for registering at gamestatsapp.com", Template.ConfirmRegistration.ToString(), confirmRegistration);
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

            await _emailService.SendEmailTemplate(user.Email, "Reset your gamestatsapp.com password", Template.ResetPasswordEmail.ToString(), passwordReset);
        }
        
        public ChangePasswordViewModel GetChangePassword(string email, long expirationTime, string token)
        {
            var user = _userRepo.GetUsers(i => i.Email == email).FirstOrDefault();
            var hashKey = _config.GetSection("SiteSettings").GetSection("HashKey").Value;
            var strToHash = string.Format("email={0}&expirationTime={1}&password={2}", email, expirationTime, user.Password);
            var hash = strToHash.GetHMACSHA256Hash(hashKey);
            var expireDate = new DateTime(expirationTime);
            var isValid = (hash == token) && expireDate > DateTime.UtcNow;
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
                IsDarkTheme = isdarktheme
            };

            _userRepo.SaveUserSetting(userSetting);      

            var userGameLists = _userRepo.GetDefaultGameLists().Select(i => new UserGameList() { UserID = user.ID, Name = i.Name, DefaultGameListID = i.ID }).ToList();
            
            _userRepo.SaveUserGameLists(userGameLists);      
        }

        public void ChangeUserPassword(string email, string pass)
        {
            var user = _userRepo.GetUsers(i => i.Email == email).FirstOrDefault();
            user.Password = pass.HashString();
            user.ModifiedBy = user.ID;
            user.ModifiedDate = DateTime.UtcNow;

            _userRepo.SaveUser(user);
        }

        public void ChangeUsername(string email, string username)
        {
            var user = _userRepo.GetUsers(i => i.Email == email).FirstOrDefault();
            user.Username = username; 
            user.ModifiedBy = user.ID;
            user.ModifiedDate = DateTime.UtcNow;

            _userRepo.SaveUser(user);
        }        

        public void SaveUserGameAccount(int userID, int gameAccountTypeID, TokenResponse tokenResponse)
        {
            var userGameAccount = _userRepo.GetUserGameAccounts(i => i.UserID == userID && i.GameAccountTypeID == gameAccountTypeID).FirstOrDefault();

            if (userGameAccount == null)
            {
                userGameAccount = new UserGameAccount()
                {
                    UserID = userID,
                    GameAccountTypeID = gameAccountTypeID,
                    AccountUserID = tokenResponse.AccountUserID,
                    AccountUserHash = tokenResponse.AccountUserHash,                   
                    CreatedDate = DateTime.UtcNow
                };
            }
            else
            {
                userGameAccount.AccountUserID = tokenResponse.AccountUserID;
                userGameAccount.AccountUserHash = tokenResponse.AccountUserHash;
                userGameAccount.ModifiedDate = DateTime.UtcNow;
            }

            var userGameAccountTokens = new List<UserGameAccountToken>() {
                new UserGameAccountToken() {
                        TokenTypeID = (int)TokenType.Access,
                        Token = tokenResponse.Token,
                        IssuedDate = tokenResponse.IssuedDate,
                        ExpireDate = tokenResponse.ExpireDate
                    } 
                };

            if (!string.IsNullOrWhiteSpace(tokenResponse.RefreshToken))
            {
                userGameAccountTokens.Add(new UserGameAccountToken() {
                    TokenTypeID = (int)TokenType.Refresh,
                    Token = tokenResponse.RefreshToken
                });
            }
            
            _userRepo.SaveUserGameAccount(userGameAccount);
            _userRepo.SaveUserGameAccountTokens(userGameAccount.ID, userGameAccountTokens);
        }

        public IEnumerable<UserGameList> GetUserGameLists (int userID)
        { 
            var userGameLists = _userRepo.GetUserGameLists(i => i.UserID == userID)
                                         .ToList();

            return userGameLists;
        }     

        public IEnumerable<GameViewModel> GetGamesByUserGameList (int userGameListID)
        { 
            var gameVMs = _userRepo.GetGamesByUserGameList(userGameListID).Select(i=>new GameViewModel(i)).ToList();

            return gameVMs;
        }                

        public void AddGameToUserGameList(int userID, int userGameListID, int gameID)
        {         
            var userGameListVM = _userRepo.GetUserGameListViews(i => i.ID == userGameListID).Select(i => new UserGameListViewModel(i)).FirstOrDefault();
                        
            if (!userGameListVM.GameIDs.Contains(gameID))
            {
                var userGameListGame = new UserGameListGame() { UserGameListID = userGameListVM.ID, GameID = gameID };
                _userRepo.SaveUserGameListGame(userGameListGame);

                if (userGameListVM.DefaultGameListID != (int)DefaultGameList.AllGames)
                {
                    var allUserGameListVM = _userRepo.GetUserGameListViews(i => i.UserID == userID && i.ID == (int)DefaultGameList.AllGames).Select(i => new UserGameListViewModel(i)).FirstOrDefault();   
                    if (!allUserGameListVM.GameIDs.Contains(gameID))
                    {
                        var allUserGameListGame = new UserGameListGame() { UserGameListID = allUserGameListVM.ID, GameID = gameID };
                        _userRepo.SaveUserGameListGame(allUserGameListGame);
                    }
                }
            }
        }        

        public void RemoveGameFromUserGameList(int userID, int userGameListID, int gameID)
        {         
            var userGameListVM = _userRepo.GetUserGameListViews(i => i.ID == userGameListID).Select(i => new UserGameListViewModel(i)).FirstOrDefault();
                        
            if (userGameListVM.GameIDs.Contains(gameID))
            {
                _userRepo.DeleteUserGameListGame(gameID, userGameListID);

                if (userGameListVM.DefaultGameListID != (int)DefaultGameList.AllGames)
                {
                    var allUserGameListVM = _userRepo.GetUserGameListViews(i => i.UserID == userID && i.ID == (int)DefaultGameList.AllGames).Select(i => new UserGameListViewModel(i)).FirstOrDefault();   
                    if (allUserGameListVM.GameIDs.Contains(gameID))
                    {
                        _userRepo.DeleteUserGameListGame(gameID, allUserGameListVM.ID);
                    }
                }
            }
        }

        public void RemoveGameFromAllUserGameLists(int userID, int gameID)
        {         
            var userGameListIDs = _userRepo.GetUserGameListViews(i => i.UserID == userID)
                                          .Select(i => new UserGameListViewModel(i))
                                          .Where(i => i.GameIDs.Contains(gameID))
                                          .Select(i => i.ID)
                                          .ToList();

            foreach(var userGameListID in userGameListIDs)
            {
                _userRepo.DeleteUserGameListGame(userGameListID, gameID);
            }
        }

        public IEnumerable<UserGameAccountViewModel> GetUserGameAccounts(int userID)
        {
            var userGameAccountVMs = _userRepo.GetUserGameAccountViews(i => i.UserID == userID).Select(i => new UserGameAccountViewModel(i)).ToList();

            return userGameAccountVMs;
        }

        public async Task<Tuple<UserGameAccountView, string>> GetRefreshedUserGameAccount(int userID, int userGameAccountID)
        {
            var authUrl = string.Empty;
            var redirectUrl = string.Empty;
            var userGameAccountVW = _userRepo.GetUserGameAccountViews(i => i.ID == userGameAccountID).FirstOrDefault();
            
            switch (userGameAccountVW.GameAccountTypeID)
            {
                case (int)GameAccountType.Xbox:
                    redirectUrl = _config.GetSection("Auth").GetSection("Microsoft").GetSection("ImportGamesRedirectUri").Value;
                    break;
            }
            
            if (userGameAccountVW.ExpireDate < DateTime.UtcNow)
            {
                if (!string.IsNullOrWhiteSpace(userGameAccountVW.RefreshToken))
                {
                    var tokenResponse = await _authService.ReAuthenticate(userGameAccountVW.RefreshToken);
                    SaveUserGameAccount(userID, userGameAccountVW.GameAccountTypeID, tokenResponse);
                }
                else
                {
                    authUrl = redirectUrl;
                }          
            }

            return new Tuple<UserGameAccountView, string>(userGameAccountVW, authUrl);
        }

        public async Task ImportGamesFromUserGameAccount(int userID, UserGameAccountView userGameAccountVW, bool isImportAll)
        {
            var gameNames = new List<string>();

            if (userGameAccountVW.GameAccountTypeID == (int)GameAccountType.Xbox)
            {
                var lastImportDate = isImportAll ? null : userGameAccountVW.ImportLastRunDate;
                gameNames = await _authService.GetUserGameNames(userGameAccountVW.AccountUserHash, userGameAccountVW.Token, Convert.ToUInt64(userGameAccountVW.AccountUserID), lastImportDate);
            }

            var gameIDs = new List<int>();
            var maxBatchCount = 500;
            var batchCount = 0;
            while (batchCount < gameNames.Count())
            {
                var gameNamesBatch = gameNames.Skip(batchCount).Take(maxBatchCount).ToList();
                var gameIDsBatch = _gameRepo.GetGames(i => gameNamesBatch.Contains(i.Name)).Select(i => i.ID).ToList();
                gameIDs.AddRange(gameIDsBatch);
                batchCount += maxBatchCount;
            }

            var allUserGameListID = _userRepo.GetUserGameLists(i => i.UserID == userID && i.DefaultGameListID == (int)DefaultGameList.AllGames)
                                             .Select(i => i.ID)
                                             .FirstOrDefault();
            var existingGameIDs = _userRepo.GetGamesByUserGameList(allUserGameListID).Select(i => i.ID).ToList();
            var userGameListGames = gameIDs.Where(i => !existingGameIDs.Contains(i))
                                           .Select(i => new UserGameListGame() { UserGameListID = allUserGameListID, GameID = i })
                                           .ToList();

            if (userGameListGames.Any())
            {
                _userRepo.SaveUserGameListGames(userGameListGames);
            }

            userGameAccountVW.ImportLastRunDate = DateTime.UtcNow;
            _userRepo.SaveUserGameAccount(userGameAccountVW.ConvertToUserGameAccount());       
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

        public IEnumerable<SearchResult> SearchUsers(string searchText)
        {
            return _userRepo.SearchUsers(searchText);
        }        
    }
}
