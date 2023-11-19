using System;
using Microsoft.AspNetCore.Mvc;
using GameStatsApp.Interfaces.Services;
using GameStatsApp.Model.ViewModels;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using GameStatsApp.Common.Extensions;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;
using GameStatsApp.Model;
using GameStatsApp.Model.JSON;
using Serilog;
using Microsoft.AspNetCore.Authorization;

namespace GameStatsApp.Controllers
{
    [Authorize]
    public class GameController : Controller
    {
        private readonly IGameService _gameService = null;
        private readonly IUserService _userService = null;
        private readonly IAuthService _authService = null;
        private readonly IConfiguration _config = null;       
        private readonly ILogger _logger = null;

        public GameController(IGameService gameService, IUserService userService, IAuthService authService, IConfiguration config, ILogger logger)
        {
            _gameService = gameService;
            _userService = userService;
            _authService = authService;    
            _config = config;
            _logger = logger;                    
        }

        [HttpGet]
        public JsonResult SearchGames(string term)
        {
            var results = _gameService.SearchGames(term);

            return Json(results);
        }

        [HttpGet]
        public JsonResult ImportGames()
        {
            var userID = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var baseUrl = string.Format("https://{0}{1}", Request.Host, Request.PathBase);
            var microsoftRedirectPath = _config.GetSection("Auth").GetSection("Microsoft").GetSection("RedirectPath").Value;         
            var microsoftRedirectUrl = string.Format("{0}{1}", baseUrl, microsoftRedirectPath);
            var steamRedirectPath = _config.GetSection("Auth").GetSection("Steam").GetSection("RedirectPath").Value;
            var steamRedirectUrl = string.Format("{0}{1}", baseUrl, steamRedirectPath);

            var importGamesVM = new ImportGamesViewModel() {
                UserAccounts = _userService.GetUserAccounts(userID).ToList(),
                MicrosoftAuthUrl = _authService.GetMicrosoftAuthUrl(microsoftRedirectUrl),
                SteamAuthUrl = _authService.GetSteamAuthUrl(steamRedirectUrl)
            };

            if (HttpContext.Session.Keys.Contains("AuthSuccess"))
            {
                importGamesVM.AuthSuccess = HttpContext.Session.Get<bool>("AuthSuccess");
                importGamesVM.AuthAccountTypeID = HttpContext.Session.Get<int>("AuthAccountTypeID");
                HttpContext.Session.Remove("AuthSuccess");
                HttpContext.Session.Remove("AuthAccountTypeID");
            }

            return Json(importGamesVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ImportGames(int userAccountID)
        {
            var success = false;
            List<string> errorMessages = new List<string>();
            var isAuthExpired = false;
            var count = 0;

            try
            {
                var userID = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var userAccountVW = await _userService.GetRefreshedUserAccount(userID, userAccountID);
                isAuthExpired = userAccountVW.ExpireDate.HasValue && userAccountVW.ExpireDate < DateTime.UtcNow;

                if (!isAuthExpired)
                {
                    if (userAccountVW.AccountTypeID == (int)AccountType.Steam && !await _authService.CheckSteamUserCommunityVisibility(userAccountVW.AccountUserID))
                    {
                        errorMessages.Add("Error importing, Steam Profile must be Public");
                    }

                    AddImportingUserAccount(userAccountVW.ID);
                    count = await _gameService.ImportGames(userID, userAccountVW);    
                    success = !errorMessages.Any();
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "ImportGames");
                success = false;
                errorMessages = new List<string>() { "Error importing games" };
            }
            
            UpdateImportingUserAccount(userAccountID, success, count, errorMessages);

            return Json(new { success, errorMessages, isAuthExpired, count });
        }

        [HttpGet]
        public ActionResult GetCompletedImportGames()
        {
            var importingUserAccounts = HttpContext.Session.Get<List<ImportingUserAccount>>("ImportingUserAccounts") ?? new List<ImportingUserAccount>();  
            var results = importingUserAccounts.Where(x => x.Success.HasValue).ToList();
            
            if (results.Any())
            {
                importingUserAccounts = importingUserAccounts.Where(x => !results.Any(i => i.UserAccountID == x.UserAccountID)).ToList();
                HttpContext.Session.Set<List<ImportingUserAccount>>("ImportingUserAccounts", importingUserAccounts);                   
            }   

            return Json(results);
        }
        
        private void AddImportingUserAccount(int userAccountID)
        {
            var importingUserAccounts = HttpContext.Session.Get<List<ImportingUserAccount>>("ImportingUserAccounts") ?? new List<ImportingUserAccount>();  
            if (!importingUserAccounts.Any(i=> i.UserAccountID == userAccountID))
            {
                importingUserAccounts.Add(new ImportingUserAccount() { UserAccountID = userAccountID });
            }
            HttpContext.Session.Set<List<ImportingUserAccount>>("ImportingUserAccounts", importingUserAccounts);
        }    

        private void UpdateImportingUserAccount(int userAccountID, bool success, int count, List<string> errorMessages)
        {
            var importingUserAccounts = HttpContext.Session.Get<List<ImportingUserAccount>>("ImportingUserAccounts") ?? new List<ImportingUserAccount>();  
            var importingUserAccount = importingUserAccounts.FirstOrDefault(i=> i.UserAccountID == userAccountID);
            if (importingUserAccount != null)
            {
                importingUserAccount.Success = success;
                importingUserAccount.Count = count;
                importingUserAccount.ErrorMessages = errorMessages;
                HttpContext.Session.Set<List<ImportingUserAccount>>("ImportingUserAccounts", importingUserAccounts);
            }
        }
    }
}
