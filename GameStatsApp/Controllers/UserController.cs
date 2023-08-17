using System;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;
using GameStatsApp.Interfaces.Services;
using GameStatsApp.Interfaces.Repositories;
using GameStatsApp.Model;
using GameStatsApp.Model.Data;
using GameStatsApp.Model.ViewModels;
using GameStatsApp.Common.Extensions;
using System.Security.Claims;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Threading.Tasks;
using System.Linq;
using Serilog;
using Microsoft.AspNetCore.Authorization;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Http.Extensions;
using Newtonsoft.Json;

namespace GameStatsApp.Controllers
{
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
        public JsonResult GetUserLists()
        {
            var userID = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));          
            var userLists = _userService.GetUserLists(userID);

            return Json(userLists);
        }

        [HttpGet]
        public JsonResult GetUserListGames(int userListID)
        {
            var userListGames = _userService.GetUserListGames(userListID);

            return Json(userListGames);
        }        
        
        [HttpPost]
        public JsonResult AddGameToUserList(int userListID, int gameID)
        {
            var success = false;
            List<string> errorMessages = null;

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

            return Json(new { success = success, errorMessages = errorMessages });
        }

        [HttpPost]
        public JsonResult RemoveGameFromUserList(int userListID, int gameID)
        {
            var success = false;
            List<string> errorMessages = null;

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

            return Json(new { success = success, errorMessages = errorMessages });
        }   

        [HttpPost]
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

        [HttpGet]
        [Authorize]
        public ViewResult UserSettings(bool? authSuccess = null)
        {
            var userID = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var userVW = _userService.GetUserViews(i => i.UserID == userID).FirstOrDefault();
            var accountTypeIDs = !string.IsNullOrWhiteSpace(userVW.AccountTypeIDs) ? userVW.AccountTypeIDs.Split(",").Select(i => int.Parse(i)).ToList() : new List<int>();
            var redirectUri = _config.GetSection("Auth").GetSection("Microsoft").GetSection("WelcomeRedirectUri").Value;
            var windowsLiveAuthUrl = _authService.GetWindowsLiveAuthUrl(redirectUri);

            var userSettingsVM = new UserSettingsViewModel() { UserID = userID,
                                                    Username = userVW.Username,                                              
                                                     WindowsLiveAuthUrl = windowsLiveAuthUrl,
                                                     AccountTypeIDs = accountTypeIDs,
                                                     AuthSuccess = authSuccess };

            return View(userSettingsVM);
        }       

        [HttpPost]
        public JsonResult SaveUserList(SaveUserListViewModel saveUserListVM)
        {
            var success = false;
            List<string> errorMessages = null;

            try
            {
                var userID = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
                if (_userService.UserListNameExists(userID, saveUserListVM.UserListName))
                {
                    ModelState.AddModelError("SaveUserList", "List name already exists");
                }

                if (ModelState.IsValid)
                {
                    _userService.SaveUserList(userID, saveUserListVM.UserListID, saveUserListVM.UserListName);
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

        public IActionResult UserListNameNotExists(string userListName)
        {
            var userID = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var result = !_userService.UserListNameExists(userID, userListName);

            return Json(result);
        }                                                   
    }
}
