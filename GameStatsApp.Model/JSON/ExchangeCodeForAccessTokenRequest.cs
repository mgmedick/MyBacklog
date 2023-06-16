using System;
using System.Collections.Generic;
using System.Linq;

namespace GameStatsApp.Model.JSON
{
    public class ExchangeCodeForAccessTokenRequest
    {
        public string code { get; set; } 
        public string client_id { get; set; }
        public string grant_type { get; set; }
        public string redirect_uri { get; set; }
        public string scope { get; set; }
    }
} 



