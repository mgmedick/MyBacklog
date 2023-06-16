using System;
using System.Collections.Generic;
using System.Linq;

namespace GameStatsApp.Model.JSON
{
    public class XASURequest
    {
		public string RelyingParty { get; set; }
        public string TokenType { get; set; }
        public XASUProperties Properties { get; set; }
	}
} 



