using System;
using System.Collections.Generic;
using System.Linq;


namespace GameStatsApp.Model.Data
{
    public class UserAccountView
    {
        public int ID { get; set; }
        public int UserID { get; set; }
        public int AccountTypeID { get; set; }
        public string AccountTypeName { get; set; }
        public string AccountUserID { get; set; }
        public string AccountUserHash { get; set; }
        public string Token { get; set; }
        public DateTime? ExpireDate { get; set; }
        public string RefreshToken { get; set; }
        public DateTime? ImportLastRunDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        
        public UserAccount ConvertToUserAccount()
        {
            return new UserAccount {
                ID = ID,
                UserID = UserID,
                AccountTypeID = AccountTypeID,
                AccountUserID = AccountUserID,
                AccountUserHash = AccountUserHash,
                ImportLastRunDate = ImportLastRunDate,
                CreatedDate = CreatedDate,
                ModifiedDate = ModifiedDate
            };
        }        
    }
} 
