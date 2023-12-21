﻿using System;
using GameStatsApp.Model.Data;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using GameStatsApp.Model;

namespace GameStatsApp.Model.ViewModels
{
    public class SignUpViewModel
    {
        public string GClientID { get; set; }
        public string FBClientID { get; set; }
        public string FBApiVer { get; set; }
        public string RecaptchaKey { get; set; }
                        
        [Required(AllowEmptyStrings = false, ErrorMessage = "Email required")]
        [EmailAddress(ErrorMessage = "Invalid Email format")]
        public string Email { get; set; }
        public string Token { get; set; }
    }
}

