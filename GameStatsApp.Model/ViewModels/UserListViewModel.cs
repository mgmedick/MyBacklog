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
        
        public UserListViewModel(UserList userList)
        {
            ID = userList.ID;
            Name = userList.Name;
            DefaultListID = userList.DefaultListID;
        }

        public int ID { get; set; }
        public string Name { get; set; }
        public int? DefaultListID { get; set; }
    }
}

