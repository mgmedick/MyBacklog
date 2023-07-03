using System;
using Microsoft.AspNetCore.Mvc;
using GameStatsApp.Interfaces.Services;
using GameStatsApp.Model;
using GameStatsApp.Model.ViewModels;
using System.Collections.Generic;

namespace GameStatsApp.Controllers
{
    public class GameController : Controller
    {
        private readonly IGameService _gameService = null;

        public GameController(IGameService gameService)
        {
            _gameService = gameService;
        }

        [HttpGet]
        public JsonResult SearchGames(string term)
        {
            var results = _gameService.SearchGames(term);

            return Json(results);
        }         
    }
}
