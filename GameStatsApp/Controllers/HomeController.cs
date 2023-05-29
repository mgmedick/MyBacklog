﻿using System;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;
using GameStatsApp.Interfaces.Services;
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

namespace GameStatsApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUserService _userService = null;
        private readonly ILogger _logger = null;

        public HomeController(IUserService userService, ILogger logger)
        {
            _userService = userService;
            _logger = logger;
        }

        public ActionResult Index()
        {
            var identity = (ClaimsIdentity)HttpContext.User.Identity;
            if (identity == null || !identity.IsAuthenticated)
            {
                return RedirectToAction("Login");
            }

            return View();
        }

        public ViewResult Error()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Login(LoginViewModel loginVM)
        {
            var success = false;
            List<string> errorMessages = null;

            try
            {
                if (!_userService.UsernameExists(loginVM.Username, true))
                {
                    ModelState.AddModelError("Login", "Invalid username");
                }

                if (!_userService.PasswordMatches(loginVM.Password, loginVM.Username))
                {
                    ModelState.AddModelError("Login", "Invalid password");
                }

                if(ModelState.IsValid)
                {
                    var userVW = _userService.GetUserViews(i => i.Username == loginVM.Username).FirstOrDefault();
                    LoginUser(userVW);
                    success = true;
                }
                else
                {
                    success = false;
                    errorMessages = ModelState.Values.SelectMany(i => i.Errors).Select(i => i.ErrorMessage).ToList();
                }
            }
            catch(Exception ex)
            {
                _logger.Error(ex, "Login");
                success = false;
                errorMessages = new List<string>() { "Error logging user in." };
            }

            return Json(new { success = success, errorMessages = errorMessages });
        }

        [HttpGet]
        public async Task<ActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            var baseUrl = string.Format("{0}://{1}{2}", Request.Scheme, Request.Host, Request.PathBase);
            var refUrl = Request.Headers["Referer"].ToString();
            var url = !string.IsNullOrWhiteSpace(refUrl) ? refUrl : baseUrl;
            
            return Redirect(url);
        }

        [HttpGet]
        public ActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public JsonResult SignUp(SignUpViewModel signUpVM)
        {
            var success = false;
            List<string> errorMessages = null;

            try
            {
                if (_userService.EmailExists(signUpVM.Email))
                {
                    ModelState.AddModelError("SignUp", "Email already exists for another user");
                }

                if (ModelState.IsValid)
                {
                    _ = _userService.SendActivationEmail(signUpVM.Email).ContinueWith(t => _logger.Error(t.Exception, "SendActivationEmail"), TaskContinuationOptions.OnlyOnFaulted);
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
                _logger.Error(ex, "SignUp");
                return Json(new { success = false, message = "Error signing up user." });
            }

            return Json(new { success = success, errorMessages = errorMessages });
        }

        [HttpGet]
        public ViewResult Activate(string email, long expirationTime, string token)
        {
            var activateUserVM = _userService.GetActivateUser(email, expirationTime, token);
            HttpContext.Session.Set<string>("Email", email);

            return View(activateUserVM);
        }

        [HttpPost]
        public JsonResult Activate(ActivateViewModel activateUserVM)
        {
            var success = false;
            List<string> errorMessages = null;

            try
            {
                if (_userService.UsernameExists(activateUserVM.Username, false))
                {
                    ModelState.AddModelError("Activate", "Username already exists for another user");
                }

                if (ModelState.IsValid)
                {
                    _userService.CreateUser(activateUserVM.Username, activateUserVM.Password);
                    var userVW = _userService.GetUserViews(i => i.Username == activateUserVM.Username).FirstOrDefault();
                    LoginUser(userVW);
                    _ = _userService.SendConfirmRegistrationEmail(userVW.Email, userVW.Username).ContinueWith(t => _logger.Error(t.Exception, "SendConfirmRegistrationEmail"), TaskContinuationOptions.OnlyOnFaulted);
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
                _logger.Error(ex, "ResetPassword");
                success = false;
                errorMessages = new List<string>() { "Error resetting password" };
            }

            return Json(new { success = success, errorMessages = errorMessages });
        }

        [HttpGet]
        public ActionResult ResetPassword()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public JsonResult ResetPassword(ResetPasswordViewModel resetPassVM)
        {
            var success = false;
            List<string> errorMessages = null;

            try
            {
                if (!_userService.UsernameExists(resetPassVM.Username, true))
                {
                    ModelState.AddModelError("ResetPassword", "Username not found");
                }

                if (ModelState.IsValid)
                {
                    _ = _userService.SendResetPasswordEmail(resetPassVM.Username).ContinueWith(t => _logger.Error(t.Exception, "SendResetPasswordEmail"), TaskContinuationOptions.OnlyOnFaulted);
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
                _logger.Error(ex, "ResetPassword");
                success = false;
                errorMessages = new List<string>() { "Error resetting password" };
            }

            return Json(new { success = success, errorMessages = errorMessages });
        }

        [HttpGet]
        public ViewResult ChangePassword(string username, string email, long expirationTime, string token)
        {
            var changePassVM = _userService.GetChangePassword(username, email, expirationTime, token);
            HttpContext.Session.Set<string>("Username", username);

            return View(changePassVM);
        }

        [HttpPost]
        public JsonResult ChangePassword(ChangePasswordViewModel changePassVM)
        {
            var success = false;
            List<string> errorMessages = null;

            try
            {
                var username = HttpContext.Session.Get<string>("Username");
                if (_userService.PasswordMatches(changePassVM.Password, username))
                {
                    ModelState.AddModelError("ChangePassword", "Password must differ from previous password");
                }

                if (ModelState.IsValid)
                {
                    _userService.ChangeUserPassword(username, changePassVM.Password);
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
                _logger.Error(ex, "ChangePassword");
                success = false;
                errorMessages = new List<string>() { "Error changing password" };
            }

            return Json(new { success = success, errorMessages = errorMessages });
        }

        private async void LoginUser(UserView userVW)
        {
            var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.NameIdentifier, userVW.UserID.ToString()),
                            new Claim(ClaimTypes.Email, userVW.Email),
                            new Claim(ClaimTypes.Name, userVW.Username),
                            new Claim("theme", userVW.IsDarkTheme ? "theme-dark" : "theme-light")
                        };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity));
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult UsernameExists(string username)
        {
            var result = _userService.UsernameExists(username, false);

            return Json(result);
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult ActiveUsernameExists(string username)
        {
            var result = _userService.UsernameExists(username, true);

            return Json(result);
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult UsernameNotExists(string username)
        {
            var result = !_userService.UsernameExists(username, false);

            return Json(result);
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult PasswordNotMatches(string password)
        {
            var username = HttpContext.Session.Get<string>("Username");
            var result = !_userService.PasswordMatches(password, username);

            return Json(result);
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult PasswordMatches(string password, string username)
        {
            var result = _userService.PasswordMatches(password, username);

            return Json(result);
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult EmailNotExists(string email)
        {
            var result = !_userService.EmailExists(email);

            return Json(result);
        }
    }
}
