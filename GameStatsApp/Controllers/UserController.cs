using System;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;
using GameStatsApp.Interfaces.Services;
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
        private readonly ILogger _logger = null;

        public UserController(IUserService userService, ILogger logger)
        {
            _userService = userService;
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

        [HttpPost]
        public JsonResult ImportGamesFromUserGameAccounts()
        {
            var success = false;
            List<string> errorMessages = null;

            try
            {
                var userID = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
                _userService.ImportGamesFromUserGameAccounts(userID);
                success = true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "ImportGamesFromUserGameAccounts");
                success = false;
                errorMessages = new List<string>() { "Error adding game to list" };
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
