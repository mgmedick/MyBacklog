using System;
using System.Collections.Generic;
using System.Linq;

namespace GameStatsApp.Model.Data
{
    public class UserGameAccountToken
    {
        public int ID { get; set; }
        public int UserGameAccountID { get; set; }
        public int TokenTypeID { get; set; }
        public string Token { get; set; }
        public DateTime? IssuedDate { get; set; }
        public DateTime? ExpireDate { get; set; }    
    }
} 
