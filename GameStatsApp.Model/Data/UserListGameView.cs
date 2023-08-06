using System;
using System.Collections.Generic;
using System.Linq;

namespace GameStatsApp.Model.Data
{
    public class UserListGameView
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int UserListGameID { get; set; }
        public string CoverImagePath { get; set; }
        public string UserListIDs { get; set; }        
    }
} 
