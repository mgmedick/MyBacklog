using System.Collections.Generic;

namespace GameStatsApp.Model
{
    public class SearchResult
    {
        public string Label { get; set; }
        public string Value { get; set; }
        public string CoverImagePath { get; set; }
        public List<SearchResult> SubItems { get; set; } 
    }
}
