using System;
using System.Linq;
using System.Collections.Generic;
using GameStatsApp.Model.Data;

namespace GameStatsApp.Model.ViewModels
{
    public class UserViewModel
    {
        public UserViewModel()
        {
        }

        public UserViewModel(UserView userView)
        {
            UserID = userView.UserID;
            Username = userView.Username;
            IsDarkTheme = userView.IsDarkTheme;
        }

        public int UserID { get; set; }
        public string Username { get; set; }
        public bool IsDarkTheme { get; set; }
    }
}


