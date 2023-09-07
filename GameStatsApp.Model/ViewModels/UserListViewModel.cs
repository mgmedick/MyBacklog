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
        
        public UserListViewModel(UserListView userListVW)
        {
            ID = userListVW.ID;
            Name = userListVW.Name;
            DefaultListID = userListVW.DefaultListID;
            UserAccountID = userListVW.UserAccountID;
            AccountTypeID = userListVW.AccountTypeID;
            Active = userListVW.Active;
            SortOrder = userListVW.SortOrder;
        }

        public int ID { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "List name required")]
        [RegularExpression(@"^[._()-\/#&$@+\w\s]{3,30}$", ErrorMessage = @"List name must be between 3 - 30 alphanumeric/special characters")]        
        public string Name { get; set; }
        public int? DefaultListID { get; set; }
        public int? UserAccountID { get; set; }
        public int? AccountTypeID { get; set; }
        public bool Active { get; set; }
        public int? SortOrder { get; set; }
    }
}

