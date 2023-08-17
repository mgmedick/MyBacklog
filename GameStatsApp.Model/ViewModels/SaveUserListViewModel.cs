using System;
using GameStatsApp.Model.Data;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace GameStatsApp.Model.ViewModels
{
    public class SaveUserListViewModel
    {
        public int UserListID { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "List name required")]
        [RegularExpression(@"^[._()-\/#&$@+\w\s]{3,30}$", ErrorMessage = @"List name must be between 3 - 30 alphanumeric/special characters")]
        public string UserListName { get; set; }
    }
}

