using System;
using GameStatsApp.Model.Data;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace GameStatsApp.Model.ViewModels
{
    public class ImportGamesViewModel
    {
        public List<UserAccountViewModel> UserAccounts { get; set; }        
        public bool? AuthSuccess { get; set; }
        public int? AuthAccountTypeID { get; set; }
    }
}

