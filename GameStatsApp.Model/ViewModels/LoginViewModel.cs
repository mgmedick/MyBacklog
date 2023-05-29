using System;
using GameStatsApp.Model.Data;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace GameStatsApp.Model.ViewModels
{
    public class LoginViewModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Username required")]
        //[Remote(action: "ActiveUsernameExists", controller: "GameStats", ErrorMessage = "Invalid Username")]
        public string Username { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Password required")]
        //[Remote(action: "PasswordMatches", controller: "GameStats", AdditionalFields = nameof(Username), ErrorMessage = "Invalid Password")]
        public string Password { get; set; }
    }
}

