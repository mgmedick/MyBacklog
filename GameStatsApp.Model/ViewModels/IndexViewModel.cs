using System;
using GameStatsApp.Model.Data;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace GameStatsApp.Model.ViewModels
{
    public class IndexViewModel
    {
        public bool IsAuth { get; set; }
        public string IndexDemoImagePath { get; set; }
        public string ImportDemoImagePath { get; set; }
        public string SettingsDemoImagePath { get; set; }
        public int UserID { get; set; }
        public string Username { get; set; }        
        public List<UserListViewModel> UserLists { get; set; }
        public bool IsDemo { get; set; }
        public string ReturnUrl { get; set; }
        public bool ShowWelcome { get; set; }
        public bool ShowImport { get; set; }
        public string RecaptchaKey { get; set; }
    }
}

