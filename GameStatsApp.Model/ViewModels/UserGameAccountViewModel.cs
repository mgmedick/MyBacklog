﻿using System;
using System.Collections.Generic;
using System.Linq;
using GameStatsApp.Common.Extensions;
using System.Collections.Specialized;
using Newtonsoft.Json;

namespace GameStatsApp.Model.Data
{
    public class UserGameAccountViewModel
    {
        public UserGameAccountViewModel()
        {
        }

        public UserGameAccountViewModel(UserGameAccountView userGameAccount)
        {
            ID = userGameAccount.ID;
            UserID = userGameAccount.UserID;
            GameAccountTypeID = userGameAccount.GameAccountTypeID;
            GameAccountTypeName = userGameAccount.GameAccountTypeName;
            ImportLastRunDate = userGameAccount.ImportLastRunDate;
        }
        
        public int ID { get; set; }
        public int UserID { get; set; }
        public int GameAccountTypeID { get; set; }
        public string GameAccountTypeName { get; set; }
        public DateTime? ImportLastRunDate { get; set; }
        
        public string RelativeImportLastRunDateString
        {
            get
            {
                return ImportLastRunDate?.ToRealtiveDateString(true);
            }
        }        
    }
} 
