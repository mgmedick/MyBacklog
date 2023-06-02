using System;
using System.Collections.Generic;
using System.Linq;

namespace GameStatsApp.Model.Data
{
    public class UserView
    {
        public int UserID { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public bool IsDarkTheme { get; set; }
        public bool Active { get; set; }
        public bool Deleted { get; set; }
    }
} 
