using System;
using System.Collections.Generic;
using System.Linq;

namespace GameStatsApp.Model.JSON
{
    public class XASResponse
    {
		public string Token;
		public DateTime IssueInstant;
		public DateTime NotAfter;
		public Dictionary<string, List<XboxUserInformation>> DisplayClaims;
    }
} 



