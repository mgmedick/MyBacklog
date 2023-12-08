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
    }
}
