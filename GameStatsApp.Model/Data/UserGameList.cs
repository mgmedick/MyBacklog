using System;
using System.Collections.Generic;
using System.Linq;

namespace GameStatsApp.Model.Data
{
    public class UserGameList
    {
        public int ID { get; set; }
        public int UserID { get; set; }
        public string Name { get; set; }
        public int? DefaultGameListID { get; set; }
    }
} 
