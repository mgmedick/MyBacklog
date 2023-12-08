using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace GameStatsApp.Model.JSON
{
    public class ImportGameResult
    {
        public int ImportTypeID { get; set; }
        public string UserListName { get; set; }
        public bool? Success { get; set; }
        public int Count { get; set; }
        public List<string> ErrorMessages { get; set; }
	}
} 



