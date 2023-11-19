using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace GameStatsApp.Model.JSON
{
    public class GameImportResult
    {
        public string Name { get; set; }
        public string SantizedName { get; set; }
        public int SortOrder { get; set; }
	}
} 



