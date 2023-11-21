using System;

namespace GameStatsApp.Model.Data
{
    public class Game
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public string CoverImageUrl { get; set; }  
        public string CoverImagePath { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string SantizedName { get; set; }
        public string SantizedNameNoSpace { get; set; } 
    }   
} 
