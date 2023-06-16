using System;
using System.Collections.Generic;
using System.Linq;

namespace GameStatsApp.Model.JSON
{
    public class XSTSRequest
    {
		public string RelyingParty { get; set; }
        public string TokenType { get; set; }
		public Dictionary<string, object> Properties { get; set; }
	}
} 



