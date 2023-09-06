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
            UserAccountID = userList.UserAccountID;
            Active = userList.Active;
            SortOrder = userList.SortOrder;
        }

        public int ID { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "List name required")]
        [RegularExpression(@"^[._()-\/#&$@+\w\s]{3,30}$", ErrorMessage = @"List name must be between 3 - 30 alphanumeric/special characters")]        
        public string Name { get; set; }
        public int? DefaultListID { get; set; }
        public int? UserAccountID { get; set; }
        public bool Active { get; set; }
        public int? SortOrder { get; set; }
    }
}

