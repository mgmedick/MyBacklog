using System;
using GameStatsApp.Model.Data;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace GameStatsApp.Model.ViewModels
{
    public class UserGameListViewModel
    {
        public UserGameListViewModel()
        {
        }

        public UserGameListViewModel(UserGameListView userGameListView)
        {
            ID = userGameListView.ID;
            UserID = userGameListView.UserID;
            Name = userGameListView.Name;
            DefaultGameListID = userGameListView.DefaultGameListID;
            GameVMs = JsonConvert.DeserializeObject<List<GameViewModel>>(userGameListView.Games) ?? new List<GameViewModel>();
        }

        public int ID { get; set; }
        public int UserID { get; set; }
        public string Name { get; set; }
        public int? DefaultGameListID { get; set; }
        public List<GameViewModel> GameVMs { get; set; }
    }
}

