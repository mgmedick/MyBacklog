using System;
using System.Collections.Generic;
using System.Text;
using NPoco;
//using Microsoft.Extensions.Configuration;

namespace GameStatsApp.Repository
{
    public abstract class BaseRepository
    {
        public static DatabaseFactory DBFactory { get; set; }
    }
}


