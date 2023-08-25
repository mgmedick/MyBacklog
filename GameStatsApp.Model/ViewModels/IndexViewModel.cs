using System;
using GameStatsApp.Model.Data;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace GameStatsApp.Model.ViewModels
{
    public class IndexViewModel
    {
        public bool IsAuth { get; set; }
        public int UserID { get; set; }
        public List<UserList> UserLists { get; set; }
        public ImportGamesViewModel ImportGamesVM { get; set; }
    }
}

