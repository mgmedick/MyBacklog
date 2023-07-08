using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace GameStatsApp.Model.JSON
{
    public class TokenResponse
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public string AccountUserID { get; set; }
        public string AccountUserHash { get; set; }
        public DateTime? IssuedDate { get; set; }
        public DateTime? ExpireDate { get; set; }
	}
} 



