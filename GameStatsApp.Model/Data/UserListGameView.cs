using System;
using System.Collections.Generic;
using System.Linq;

namespace GameStatsApp.Model.Data
{
    public class UserListGameView
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int UserListID { get; set; }
        public bool UserListActive { get; set; }
        public string CoverImagePath { get; set; }
        public string UserListIDs { get; set; }
        public int UserListGameID { get; set; }
        public int UserID { get; set; }
        public int SortOrder { get; set; }

        public UserListGame ConvertToUserListGame()
        {
            return new UserListGame {
                ID = UserListGameID,
                UserListID = UserListID,
                GameID = ID,
                SortOrder = SortOrder
            };
        }           
    }  
} 
