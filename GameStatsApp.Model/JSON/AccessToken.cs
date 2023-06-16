using System;
using System.Collections.Generic;
using System.Linq;

namespace GameStatsApp.Model.JSON
{
    public class AccessToken
    {
        public string Jwt { get; set; }
        public DateTime Issued { get; set; }
        public DateTime Expires { get; set; }
    }
} 



