using System;
using System.Collections.Generic;
using System.Linq;

namespace GameStatsApp.Model.Data
{
    public class UserList
    {
        public int ID { get; set; }
        public int UserID { get; set; }
        public string Name { get; set; }
        public int? DefaultListID { get; set; }
        public int? AccountTypeID { get; set; }
        public int? SortOrder { get; set; }
        public bool Active { get; set; }
        public bool Deleted { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }        
    }
} 
