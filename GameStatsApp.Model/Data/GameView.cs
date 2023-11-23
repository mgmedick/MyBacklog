using System;

namespace GameStatsApp.Model.Data
{
    public class GameView
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public string CoverImagePath { get; set; }
        public string SantizedName { get; set; }
        public string SantizedNameNoSpace { get; set; }         
    }   
} 
