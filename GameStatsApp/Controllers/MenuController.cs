using System;
using Microsoft.AspNetCore.Mvc;
using GameStatsApp.Interfaces.Services;
using GameStatsApp.Model;
using GameStatsApp.Model.ViewModels;
using System.Collections.Generic;

namespace GameStatsApp.Controllers
{
    public class MenuController : Controller
    {
        private readonly IMenuService _menuService = null;
        private readonly IUserService _userService = null;

        public MenuController(IMenuService menuService, IUserService userService)
        {
            _menuService = menuService;
            _userService = userService;
        }

        public ViewResult About()
        {
            return View();
        }

        [HttpGet]
        public JsonResult Search(string term)
        {
            var results = _menuService.Search(term);

            return Json(results);
        }         
    }
}
