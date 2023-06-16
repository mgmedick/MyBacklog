using System;
using System.Collections.Generic;
using System.Linq;

namespace GameStatsApp.Model.JSON
{
    public class WindowsLiveResponse
    {
		public string AccessToken { get; set; }
		public int ExpiresIn { get; set; }
		public string RefreshToken { get; set; }
		public string UserId { get; set; }
		public string Scope { get; set; }
		public string TokenType { get; set; }
        
		// Not part of actual response data
		public DateTime CreationTime { get; set; }

    }
} 



