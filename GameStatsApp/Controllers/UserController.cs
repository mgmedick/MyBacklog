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
        
        public async Task<ActionResult> MicrosoftAuthCallback()
        {
            var success = false;

            try
            {
                var baseUrl = string.Format("https://{0}{1}", Request.Host, Request.PathBase);
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
    }
}
