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
        public JsonResult GetUserGameLists(int userID)
        {
            var userGameLists = _userService.GetUserGameLists(userID);

            return Json(userGameLists);
        }

        [HttpGet]
        public JsonResult GetGamesByUserGameList(int userGameListID)
        {
            var userGameListGames = _userService.GetGamesByUserGameList(userGameListID);

            return Json(userGameListGames);
        }        

        // [HttpPost]
        // public JsonResult GetUserGameAccounts()
        // {
        //     var userID = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
        //     var userGameAccountVMs = _userService.GetUserGameAccounts(userID).ToList();

        //     return Json(userGameAccountVMs);
        // }

        [HttpPost]
        public async Task<ActionResult> ImportGamesFromUserGameAccount(int userGameAccountID)
        {
            var success = false;
            List<string> errorMessages = null;

            try
            {
                var userID = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var result = await _userService.GetAndReAuthUserGameAccount(userID, userGameAccountID);
                if (!string.IsNullOrWhiteSpace(result.Item2))
                {
                    Redirect(result.Item2);
                }
                else
                {
                    await _userService.ImportGamesFromUserGameAccount(userID, result.Item1);
                }

                success = true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "ImportGamesFromUserGameAccount");
                success = false;
                errorMessages = new List<string>() { "Error importing games" };
            }

            return Json(new { success = success, errorMessages = errorMessages });
        }
        
        [HttpPost]
        public JsonResult AddGameToUserGameList(int userGameListID, int gameID)
        {
            var success = false;
            List<string> errorMessages = null;

            try
            {
                var userID = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
                _userService.AddGameToUserGameList(userID, userGameListID, gameID);
                success = true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "AddGameToUserGameList");
                success = false;
                errorMessages = new List<string>() { "Error adding game to list" };
            }

            return Json(new { success = success, errorMessages = errorMessages });
        }

        [HttpPost]
        public JsonResult RemoveGameFromUserGameList(int userGameListID, int gameID)
        {
            var success = false;
            List<string> errorMessages = null;

            try
            {
                var userID = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
                _userService.RemoveGameFromUserGameList(userID, userGameListID, gameID);
                success = true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "RemoveGameFromUserGameList");
                success = false;
                errorMessages = new List<string>() { "Error removing game from list" };
            }

            return Json(new { success = success, errorMessages = errorMessages });
        }   

        [HttpPost]
        public JsonResult RemoveGameFromAllUserGameLists(int gameID)
        {
            var success = false;
            List<string> errorMessages = null;

            try
            {
                var userID = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
                _userService.RemoveGameFromAllUserGameLists(userID, gameID);
                success = true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "RemoveGameFromUserGameList");
                success = false;
                errorMessages = new List<string>() { "Error removing game from list" };
            }

            return Json(new { success = success, errorMessages = errorMessages });
        }                       
    }
}
