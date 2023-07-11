using System;
using System.Collections.Generic;
using System.Linq;

namespace GameStatsApp.Model.Data
{
    public class UserGameAccountViewModel
    {
        public UserGameAccountViewModel()
        {
        }

        public UserGameAccountViewModel(UserGameAccount userGameAccount)
        {
            ID = userGameAccount.ID;
            UserID = userGameAccount.UserID;
            GameAccountTypeID = userGameAccount.GameAccountTypeID;
            GameAccountTypeName = ((GameAccountType)GameAccountTypeID).ToString();
            Token = userGameAccount.Token;
            RefreshToken = userGameAccount.RefreshToken;
            AccountUserHash = userGameAccount.AccountUserHash;
            ExpireDate = userGameAccount.ImportLastRunDate;
            ImportLastRunDate = userGameAccount.ImportLastRunDate;
            IsImportRunning = userGameAccount.IsImportRunning;
        }
        
        public int ID { get; set; }
        public int UserID { get; set; }
        public int GameAccountTypeID { get; set; }
        public string GameAccountTypeName { get; set; }
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public string AccountUserID { get; set; }
        public string AccountUserHash { get; set; }
        public DateTime? ExpireDate { get; set; }
        public DateTime? ImportLastRunDate { get; set; }
        public bool IsImportRunning { get; set; }
    }
} 
