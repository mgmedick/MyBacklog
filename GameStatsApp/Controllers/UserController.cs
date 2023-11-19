using System;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;
using GameStatsApp.Interfaces.Services;
using GameStatsApp.Interfaces.Repositories;
using GameStatsApp.Model;
using GameStatsApp.Model.ViewModels;
using GameStatsApp.Common.Extensions;
using System.Security.Claims;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Serilog;
using Microsoft.AspNetCore.Authorization;

namespace GameStatsApp.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly IUserService _userService = null;
        private readonly IAuthService _authService = null;
        private readonly IUserRepository _userRepo = null;
        private readonly IConfiguration _config = null;       
        private readonly ILogger _logger = null;

        public UserController(IUserService userService, IAuthService authService, IUserRepository userRepo, IConfiguration config, ILogger logger)
        {
            _userService = userService;
            _authService = authService;
            _userRepo = userRepo;
            _config = config;
            _logger = logger;
        }

        [HttpGet]
        public ViewResult UserSettings()
        {
            var userID = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var userVW = _userService.GetUserViews(i => i.UserID == userID).FirstOrDefault();

            var userSettingsVM = new UserSettingsViewModel() { UserID = userID,
                                                    Username = userVW.Username,   
                                                    Email = userVW.Email };

            return View(userSettingsVM);
        }

        [HttpGet]
        public JsonResult GetUserListGames(int userListID)
        {
            var userID = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var userListGames = _userService.GetUserListGames(userID, userListID);

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
                result = _userService.AddNewGameToUserList(userID, userListID, gameID);
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
                _userService.AddGameToUserList(userID, userListID, gameID);
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
                _userService.RemoveGameFromUserList(userID, userListID, gameID);
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
                _userService.RemoveGameFromAllUserLists(userID, gameID);
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
                _userService.RemoveAllGamesFromUserList(userID, userListID);
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
      
        public async Task<ActionResult> MicrosoftAuthCallback()
        {
            var success = false;

            try
            {
                var baseUrl = string.Format("{0}://{1}{2}", Request.Scheme, Request.Host, Request.PathBase);
                var microsoftRedirectPath = _config.GetSection("Auth").GetSection("Microsoft").GetSection("RedirectPath").Value;   
                var redirectUri = string.Format("{0}{1}", baseUrl, microsoftRedirectPath);
                            
                var code = Request.Query["code"].ToString();
                if (!string.IsNullOrWhiteSpace(code))
                {
                    var tokenResponse = await _authService.AuthenticateMicrosoft(code, redirectUri);

                    var userID = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
                    _userService.SaveUserAccount(userID, (int)AccountType.Xbox, tokenResponse);

                    success = true;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "MicrosoftAuthCallback");
                success = false;
            }   

            HttpContext.Session.Set<bool>("AuthSuccess", success); 
            HttpContext.Session.Set<int>("AuthAccountTypeID", (int)AccountType.Xbox); 
            TempData.Add("ShowImport", true);

            return RedirectToAction("Index", "Home");
        }

        public async Task<ActionResult> SteamAuthCallback()
        {
            var success = false;

            try
            {
                var redirectUri = _config.GetSection("Auth").GetSection("Steam").GetSection("RedirectUri").Value;
                var results = new Dictionary<string, string>();
                foreach (var item in Request.Query)
                {
                    results.Add(item.Key, item.Value);
                }

                if (results.Any())
                {
                    var tokenResponse = await _authService.AuthenticateSteam(results);
                    var userID = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
                    _userService.SaveUserAccount(userID, (int)AccountType.Steam, tokenResponse);

                    success = true;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "SteamAuthCallback");
                success = false;
            }   

            HttpContext.Session.Set<bool>("AuthSuccess", success); 
            HttpContext.Session.Set<int>("AuthAccountTypeID", (int)AccountType.Steam); 
            TempData.Add("ShowImport", true);

            return RedirectToAction("Index", "Home");
        }
        
        [HttpGet]
        public JsonResult ManageUserLists()
        {
            var userID = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));          
            var userLists = _userService.GetUserLists(userID).ToList();

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
                if (_userService.UserListNameExists(userID, userListVM.ID, userListVM.Name))
                {
                    ModelState.AddModelError("ManageUserLists", "List name already exists");
                }

                if (ModelState.IsValid)
                {
                    _userService.SaveUserList(userID, userListVM);
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
                _userService.DeleteUserList(userID, userListID);
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
                _userService.UpdateUserListActive(userID, userListID, active);
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
                _userService.UpdateUserListSortOrders(userID, userListIDs);
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
            var result = !_userService.UserListNameExists(userID, userListID, userListName);

            return Json(result);
        }                                                   
    }
}
