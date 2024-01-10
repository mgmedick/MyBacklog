using System;
using System.Collections.Generic;
using System.Linq;

namespace GameStatsApp.Model.Data
{
    public class UserListGame
    {
        public int ID { get; set; }
        public int UserListID { get; set; }
        public int GameID { get; set; }
        public int SortOrder { get; set; }
    }
} 
