using System;
using GameStatsApp.Model.Data;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace GameStatsApp.Model.ViewModels
{
    public class WelcomeViewModel
    {
        public string Username { get; set; }
        public string WindowsLiveAuthUrl { get; set; }
        public List<int> GameAccountTypeIDs { get; set; }        
        public bool? Success { get; set; }
    }
}

