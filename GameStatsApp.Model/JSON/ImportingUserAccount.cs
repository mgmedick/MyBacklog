using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace GameStatsApp.Model.JSON
{
    public class ImportingUserAccount
    {
        public int UserAccountID { get; set; }
        public bool? Success { get; set; }
        public int Count { get; set; }
        public List<string> ErrorMessages { get; set; }
	}
} 



