using System;
using System.Collections.Generic;
using System.Linq;

namespace GameStatsApp.Model.Data
{
    public class UserGameServiceToken
    {
        public int ID { get; set; }
        public int UserID { get; set; }
        public int GameServiceID { get; set; }
        public string Token { get; set; }
        public DateTime? IssuedDate { get; set; }
        public DateTime? ExpireDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }        
    }
} 
