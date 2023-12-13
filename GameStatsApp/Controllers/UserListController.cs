using System;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;
using GameStatsApp.Interfaces.Services;
using GameStatsApp.Interfaces.Repositories;
using GameStatsApp.Model;
using GameStatsApp.Model.JSON;
using GameStatsApp.Model.ViewModels;
using GameStatsApp.Common.Extensions;
using System.Security.Claims;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Serilog;
using Microsoft.AspNetCore.Authorization;
using System.IO;

namespace GameStatsApp.Controllers
{
    [Authorize]
    public class UserListController : Controller
    {
        private readonly IUserListService _userListService = null;
        private readonly IUserService _userService = null;
        private readonly IAuthService _authService = null;
        private readonly IConfiguration _config = null;
        private readonly ILogger _logger = null;

        public UserListController(IUserListService userListService, IUserService userService, IAuthService authService, IConfiguration config, ILogger logger)
        {
            _userListService = userListService;
            _userService = userService;
            _authService = authService;
            _config = config; 
            _logger = logger;
        }

         [HttpGet]
        public JsonResult GetUserListGames(int userListID)
        {
            var userID = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var userListGames = _userListService.GetUserListGames(userID, userListID);

            return Json(userListGames);
        }        
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult AddNewGameToUserList(int userListID, int gameID)
        {
            var success = false;
            List<string> errorMessages = null;
            UserListGameViewModel result = null;

            try
            {
                var userID = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
                result = _userListService.AddNewGameToUserList(userID, userListID, gameID);
                success = true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "AddNewGameToUserList");
                success = false;
                errorMessages = new List<string>() { "Error adding game to list" };
            }

            return Json(new { success = success, errorMessages = errorMessages, result = result });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult AddGameToUserList(int userListID, int gameID)
        {
            var success = false;
            List<string> errorMessages = null;
            UserListGameViewModel result = null;

            try
            {
                var userID = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
                _userListService.AddGameToUserList(userID, userListID, gameID);
                success = true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "AddGameToUserList");
                success = false;
                errorMessages = new List<string>() { "Error adding game to list" };
            }

            return Json(new { success = success, errorMessages = errorMessages, result = result });
        }

        [HttpPost]  
        [ValidateAntiForgeryToken]
        public JsonResult RemoveGameFromUserList(int userListID, int gameID)
        {
            var success = false;
            List<string> errorMessages = null;
            UserListGameViewModel result = null;

            try
            {
                var userID = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
                _userListService.RemoveGameFromUserList(userID, userListID, gameID);
                success = true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "RemoveGameFromUserList");
                success = false;
                errorMessages = new List<string>() { "Error removing game from list" };
            }

            return Json(new { success = success, errorMessages = errorMessages, result = result });
        }   

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult RemoveGameFromAllUserLists(int gameID)
        {
            var success = false;
            List<string> errorMessages = null;

            try
            {
                var userID = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
                _userListService.RemoveGameFromAllUserLists(userID, gameID);
                success = true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "RemoveGameFromUserList");
                success = false;
                errorMessages = new List<string>() { "Error removing game from list" };
            }

            return Json(new { success = success, errorMessages = errorMessages });
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult RemoveAllGamesFromUserList(int userListID)
        {
            var success = false;
            List<string> errorMessages = null;

            try
            {
                var userID = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
                _userListService.RemoveAllGamesFromUserList(userID, userListID);
                success = true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "RemoveAllGamesFromUserList");
                success = false;
                errorMessages = new List<string>() { "Error removing games from list" };
            }

            return Json(new { success = success, errorMessages = errorMessages });
        }

        [HttpGet]
        public JsonResult ImportGames()
        {
            var baseUrl = string.Format("https://{0}{1}", Request.Host, Request.PathBase);
            var microsoftRedirectPath = _config.GetSection("Auth").GetSection("Microsoft").GetSection("RedirectPath").Value;         
            var microsoftRedirectUrl = string.Format("{0}{1}", baseUrl, microsoftRedirectPath);
            var steamRedirectPath = _config.GetSection("Auth").GetSection("Steam").GetSection("RedirectPath").Value;
            var steamRedirectUrl = string.Format("{0}{1}", baseUrl, steamRedirectPath);

            var importGamesVM = new ImportGamesViewModel() {
                MicrosoftAuthUrl = _authService.GetMicrosoftAuthUrl(microsoftRedirectUrl),
                SteamAuthUrl = _authService.GetSteamAuthUrl(steamRedirectUrl)
            };

            if (HttpContext.Session.Keys.Contains("AuthSuccess"))
            {
                importGamesVM.AuthSuccess = HttpContext.Session.Get<bool>("AuthSuccess");
                importGamesVM.AuthImportTypeID = HttpContext.Session.Get<int>("AuthImportTypeID");
                HttpContext.Session.Remove("AuthSuccess");
                HttpContext.Session.Remove("AuthImportTypeID");
            }

            return Json(importGamesVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> ImportGamesFromFile(int userListID)
        {
            var success = false;
            var count = 0;
            var errorMessages = new List<string>();
            var importTypeID = (int)ImportType.File;

            try
            {
                var file = Request.Form.Files.FirstOrDefault();
                var userID = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var userList = _userListService.GetUserLists(userID).FirstOrDefault(i => i.ID == userListID);
                var extensions = _config.GetSection("SiteSettings").GetSection("AllowedUploadExtensions").Value.Split(",").ToList();

                if (file != null && userList != null)
                {
                    if (!extensions.Contains(Path.GetExtension(file.FileName), StringComparer.OrdinalIgnoreCase))
                    {
                        errorMessages.Add("Error importing, incorrect file extension");
                    }
                    else
                    {
                        AddImportGameResult(importTypeID, userList.Name);
                        count = await _userListService.ImportGamesFromFile(userListID, file);
                        success = true;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "ImportGamesFromFile");
                success = false;
                errorMessages = new List<string>() { "Error importing games" };
            }            
            UpdateImportGameResult(importTypeID, success, count, errorMessages);

            return Json(new { success, count, errorMessages });    
        }       

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> ImportGamesFromUserAccount(int importTypeID, int userListID)
        {
            var success = false;
            var isAuthExpired = false;
            var count = 0;
            var errorMessages = new List<string>();

            try
            {
                var accountTypeID = 0;
                switch(importTypeID)
                {
                    case (int)ImportType.Steam:
                        accountTypeID = (int)AccountType.Steam;
                        break;
                    case (int)ImportType.Xbox:
                        accountTypeID = (int)AccountType.Xbox;
                        break;
                }

                var userID = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var userAccountVW = await _userService.GetRefreshedUserAccount(userID, accountTypeID);
                var userList = _userListService.GetUserLists(userID).FirstOrDefault(i => i.ID == userListID);

                if (userAccountVW != null && userList != null)
                {
                    if (userAccountVW.ExpireDate.HasValue && userAccountVW.ExpireDate < DateTime.UtcNow)
                    {
                        isAuthExpired = true;
                    }
                    else if (userAccountVW.AccountTypeID == (int)AccountType.Steam && !await _authService.CheckSteamUserCommunityVisibility(userAccountVW.AccountUserID))
                    {
                        errorMessages.Add("Error importing, Steam Profile must be Public");
                    }
                    else
                    {    
                        AddImportGameResult(importTypeID, userList.Name);
                        count = await _userListService.ImportGamesFromUserAccount(userListID, userAccountVW);
                        success = true;
                    }         
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "ImportGamesFromUserAccount");
                success = false;
                errorMessages = new List<string>() { "Error importing games" };
            }            
            UpdateImportGameResult(importTypeID, success, count, errorMessages);

            return Json(new { success, isAuthExpired, count, errorMessages });
        }
        
        private void AddImportGameResult(int importTypeID, string userListName)
        {
            var importResults = HttpContext.Session.Get<List<ImportGameResult>>("ImportGameResults") ?? new List<ImportGameResult>();  
            if (!importResults.Any(i=> i.ImportTypeID == importTypeID))
            {
                importResults.Add(new ImportGameResult() { ImportTypeID = importTypeID, UserListName = userListName });
            }
            HttpContext.Session.Set<List<ImportGameResult>>("ImportGameResults", importResults);
        }    

        private void UpdateImportGameResult(int importTypeID, bool success, int count, List<string> errorMessages)
        {
            var importResults = HttpContext.Session.Get<List<ImportGameResult>>("ImportGameResults") ?? new List<ImportGameResult>();  
            var importResult = importResults.FirstOrDefault(i=> i.ImportTypeID == importTypeID);
            if (importResult != null)
            {
                importResult.Success = success;
                importResult.Count = count;
                importResult.ErrorMessages = errorMessages;
                HttpContext.Session.Set<List<ImportGameResult>>("ImportGameResults", importResults);
            }
        }    

        [HttpGet]
        public JsonResult GetCompletedImportGameResults()
        {
            var importResults = HttpContext.Session.Get<List<ImportGameResult>>("ImportGameResults") ?? new List<ImportGameResult>();  
            var results = importResults.Where(x => x.Success.HasValue).ToList();
            
            if (results.Any())
            {
                importResults = importResults.Where(x => !results.Any(i => i.ImportTypeID == x.ImportTypeID)).ToList();
                HttpContext.Session.Set<List<ImportGameResult>>("ImportGameResults", importResults);                   
            }   

            return Json(results);
        }               

        [HttpGet]
        public JsonResult ManageUserLists()
        {
            var userID = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));          
            var userLists = _userListService.GetUserLists(userID).ToList();

            return Json(userLists);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult ManageUserLists(UserListViewModel userListVM)
        {
            var success = false;
            List<string> errorMessages = null;

            try
            {
                var userID = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
                if (_userListService.UserListNameExists(userID, userListVM.ID, userListVM.Name))
                {
                    ModelState.AddModelError("ManageUserLists", "List name already exists");
                }

                if (ModelState.IsValid)
                {
                    _userListService.SaveUserList(userID, userListVM);
                    success = true;
                }
                else
                {
                    success = false;
                    errorMessages = ModelState.Values.SelectMany(i => i.Errors).Select(i => i.ErrorMessage).ToList();
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "SaveUserList");
                success = false;
                errorMessages = new List<string>() { "Error saving user list" };
            }

            return Json(new { success = success, errorMessages = errorMessages });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult DeleteUserList(int userListID)
        {
            var success = false;
            List<string> errorMessages = null;

            try
            {
                var userID = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
                _userListService.DeleteUserList(userID, userListID);
                success = true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "DeleteUserList");
                success = false;
                errorMessages = new List<string>() { "Error deleting user list" };
            }

            return Json(new { success = success, errorMessages = errorMessages });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult UpdateUserListActive(int userListID, bool active)
        {
            var success = false;
            List<string> errorMessages = null;

            try
            {
                var userID = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
                _userListService.UpdateUserListActive(userID, userListID, active);
                success = true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "UpdateUserListActive");
                success = false;
                errorMessages = new List<string>() { "Error updating user list" };
            }

            return Json(new { success = success, errorMessages = errorMessages });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult UpdateUserListSortOrders(List<int> userListIDs)
        {
            var success = false;
            List<string> errorMessages = null;

            try
            {
                var userID = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
                _userListService.UpdateUserListSortOrders(userID, userListIDs);
                success = true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "UpdateUserListSortOrders");
                success = false;
                errorMessages = new List<string>() { "Error updating user list" };
            }

            return Json(new { success = success, errorMessages = errorMessages });
        }

        [HttpGet]
        public IActionResult UserListNameNotExists(int userListID, string userListName)
        {
            var userID = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var result = !_userListService.UserListNameExists(userID, userListID, userListName);

            return Json(result);
        }                                                     
    }
}
