using System;
using GameStatsApp.Model.Data;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace GameStatsApp.Model.ViewModels
{
    public class ChangeUsernameViewModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Username required")]
        [RegularExpression(@"^[._()-\/#&$@+\w\s]{3,30}$", ErrorMessage = @"Username must be between 3 - 30 alphanumeric/special characters")]
        public string Username { get; set; }
    }
}

