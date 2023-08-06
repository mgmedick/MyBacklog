using System;
using GameStatsApp.Model.Data;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace GameStatsApp.Model.ViewModels
{
    public class UserListViewModel
    {
        public UserListViewModel()
        {
        }

        public UserListViewModel(UserListView userListView)
        {
            ID = userListView.ID;
            UserID = userListView.UserID;
            Name = userListView.Name;
            DefaultListID = userListView.DefaultListID;
            GameIDs = !string.IsNullOrWhiteSpace(userListView.GameIDs) ? userListView.GameIDs.Split(',').Select(i => Convert.ToInt32(i)).ToList() : new List<int>();
        }

        public int ID { get; set; }
        public int UserID { get; set; }
        public string Name { get; set; }
        public int? DefaultListID { get; set; }
        public List<int> GameIDs { get; set; }
    }
}

