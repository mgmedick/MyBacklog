﻿using System;
using GameStatsApp.Model.Data;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace GameStatsApp.Model.ViewModels
{
    public class ChangePasswordViewModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Password required")]
        [Compare(otherProperty: nameof(ConfirmPassword))]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)[._()-\/#&$@+\w\s]{8,30}$", ErrorMessage = @"Password must be between 8 - 30 alphanumeric/special characters with 1 number and letter")]
        public string Password { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Confirm Password required")]
        public string ConfirmPassword { get; set; }

        public bool IsValid { get; set; }
    }
}

