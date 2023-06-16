using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace GameStatsApp.Model.JSON
{
    public class XboxUserInformation
    {
		[JsonProperty(PropertyName="agg")]
		public string AgeGroup;

		[JsonProperty(PropertyName="gtg")]
		public string Gamertag;

		[JsonProperty(PropertyName="prv")]
		public string Privileges;

		[JsonProperty(PropertyName="usr")]
		public string UserSettingsRestrictions;

		[JsonProperty(PropertyName="utr")]
		public string UserTitleRestrictions;

		[JsonProperty(PropertyName="xid")]
		public ulong XboxUserId;

		[JsonProperty(PropertyName="uhs")]
		public string Userhash;
	}
} 



