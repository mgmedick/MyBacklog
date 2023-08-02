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
        public string GClientID { get; set; }
        public string FBClientID { get; set; }
        public string FBApiVer { get; set; }
        
        [Required(AllowEmptyStrings = false, ErrorMessage = "Email required")]
        public string Email { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Password required")]
        public string Password { get; set; }
    }
}

