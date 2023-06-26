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
    }
}
