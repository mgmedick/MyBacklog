using System;
using System.Collections.Generic;
using System.Linq;


namespace GameStatsApp.Model.Data
{
    public class UserGameAccountView
    {
        public int ID { get; set; }
        public int UserID { get; set; }
        public int GameAccountTypeID { get; set; }
        public string GameAccountTypeName { get; set; }
        public string AccountUserID { get; set; }
        public string AccountUserHash { get; set; }
        public string Token { get; set; }
        public DateTime? ExpireDate { get; set; }
        public string RefreshToken { get; set; }
        public DateTime? ImportLastRunDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        
        public UserGameAccount ConvertToUserGameAccount()
        {
            return new UserGameAccount {
                ID = ID,
                UserID = UserID,
                GameAccountTypeID = GameAccountTypeID,
                AccountUserID = AccountUserID,
                AccountUserHash = AccountUserHash,
                ImportLastRunDate = ImportLastRunDate,
                CreatedDate = CreatedDate,
                ModifiedDate = ModifiedDate
            };
        }        
    }
} 
