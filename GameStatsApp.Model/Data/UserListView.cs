using System;
using System.Collections.Generic;
using System.Linq;

namespace GameStatsApp.Model.Data
{
    public class UserListView
    {
        public int ID { get; set; }
        public int UserID { get; set; }
        public string Name { get; set; }
        public int? DefaultListID { get; set; }
        public int? UserAccountID { get; set; }
        public int? AccountTypeID { get; set; } 
        public bool Active { get; set; }
        public int? SortOrder { get; set; }
    }
} 
