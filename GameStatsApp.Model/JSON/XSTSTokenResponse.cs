using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace GameStatsApp.Model.JSON
{
    public class XSTSTokenResponse
    {
		public string Token;
		public DateTime IssueInstant;
		public DateTime NotAfter;

		[JsonProperty(PropertyName="DisplayClaims.uid")]
		public XboxUserInformation UserInformation;
	}
} 



