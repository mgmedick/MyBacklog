﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace GameStatsApp.Model.Data
{
    public class GameView
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string CoverImagePath { get; set; }
        public string UserListIDs { get; set; }
        public int UserListGameID { get; set; }
    }
} 
