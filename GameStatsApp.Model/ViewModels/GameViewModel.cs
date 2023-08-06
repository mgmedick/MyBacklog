using System;
using System.Linq;
using System.Collections.Generic;
using GameStatsApp.Model.Data;

namespace GameStatsApp.Model.ViewModels
{
    public class GameViewModel
    {
       public GameViewModel()
        {
        }
        
        public GameViewModel(GameView gameVW)
        {
            ID = gameVW.ID;
            Name = gameVW.Name;
            CoverImagePath = gameVW.CoverImagePath;
            UserListIDs = gameVW.UserListIDs.Split(",").Select(i => Convert.ToInt32(i)).ToList();
            UserListGameID = gameVW.UserListGameID;
        }

        public int ID { get; set; }
        public string Name { get; set; }
        public string CoverImagePath { get; set; }
        public List<int> UserListIDs { get; set; }
        public int UserListGameID { get; set; }
    }
}


