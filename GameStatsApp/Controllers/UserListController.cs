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
    public class UserListController : Controller
    {
        private readonly IUserListService _userListService = null;
        private readonly ILogger _logger = null;

        public UserListController(IUserListService userListService, ILogger logger)
        {
            _userListService = userListService;
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
