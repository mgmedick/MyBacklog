using System;
using System.Collections.Generic;
using System.Linq;

namespace GameStatsApp.Model.Data
{
    public class UserGameAccount
    {
        public int ID { get; set; }
        public int UserID { get; set; }
        public int GameAccountTypeID { get; set; }
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public string AccountUserID { get; set; }
        public string AccountUserHash { get; set; }
        public DateTime? IssuedDate { get; set; }
        public DateTime? ExpireDate { get; set; }
        public DateTime? ImportLastRunDate { get; set; }
        public bool IsImportRunning { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }        
    }
} 
