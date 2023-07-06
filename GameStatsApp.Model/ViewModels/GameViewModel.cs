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
            UserGameListIDs = gameVW.UserGameListIDs.Split(",").Select(i => Convert.ToInt32(i)).ToList();
        }

        public int ID { get; set; }
        public string Name { get; set; }
        public string CoverImagePath { get; set; }
        public List<int> UserGameListIDs { get; set; }
    }
}


