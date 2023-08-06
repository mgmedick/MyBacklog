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
        public JsonResult GetUserLists(int userID)
        {
            var userLists = _userService.GetUserLists(userID);

            return Json(userLists);
        }

        [HttpGet]
        public JsonResult GetGamesByUserList(int userListID)
        {
            var userListGames = _userService.GetGamesByUserList(userListID);

            return Json(userListGames);
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
    }
}
