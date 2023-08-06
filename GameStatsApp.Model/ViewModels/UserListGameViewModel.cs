using System;
using System.Linq;
using System.Collections.Generic;
using GameStatsApp.Model.Data;

namespace GameStatsApp.Model.ViewModels
{
    public class UserListGameViewModel
    {
        public UserListGameViewModel(UserListGameView userListGameVW)
        {
            ID = userListGameVW.ID;
            Name = userListGameVW.Name;
            CoverImagePath = userListGameVW.CoverImagePath;
            UserListIDs = userListGameVW.UserListIDs.Split(",").Select(i => Convert.ToInt32(i)).ToList();
            UserListGameID = userListGameVW.UserListGameID;
        }

        public int ID { get; set; }
        public string Name { get; set; }
        public string CoverImagePath { get; set; }
        public List<int> UserListIDs { get; set; }
        public int UserListGameID { get; set; }
    }
}


