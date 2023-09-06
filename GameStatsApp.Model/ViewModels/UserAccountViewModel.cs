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
            UserListID = userAccount.UserListID;
            UserListName = userAccount.UserListName;
            ImportLastRunDate = userAccount.ImportLastRunDate;
        }
        
        public int ID { get; set; }
        public int UserID { get; set; }
        public int AccountTypeID { get; set; }
        public int UserListID { get; set; }
        public string UserListName { get; set; }
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
