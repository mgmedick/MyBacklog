using System;
using System.Collections.Generic;
using System.Linq;
using GameStatsApp.Common.Extensions;
using System.Collections.Specialized;
using Newtonsoft.Json;

namespace GameStatsApp.Model.Data
{
    public class UserAccountViewModel
    {
        public UserAccountViewModel()
        {
        }

        public UserAccountViewModel(UserAccountView userAccount)
        {
            ID = userAccount.ID;
            UserID = userAccount.UserID;
            AccountTypeID = userAccount.AccountTypeID;
            AccountTypeName = userAccount.AccountTypeName;
            ImportLastRunDate = userAccount.ImportLastRunDate;
        }
        
        public int ID { get; set; }
        public int UserID { get; set; }
        public int AccountTypeID { get; set; }
        public string AccountTypeName { get; set; }
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
