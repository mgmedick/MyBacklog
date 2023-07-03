using System;
using System.Collections.Generic;
using NPoco;
using Serilog;
using NPoco.Extensions;
using System.Linq;
using GameStatsApp.Model;
using GameStatsApp.Model.Data;
using GameStatsApp.Interfaces.Repositories;
using Microsoft.Extensions.Configuration;
using System.Linq.Expressions;
using System.Collections;

namespace GameStatsApp.Repository
{
    public class GameRespository : BaseRepository, IGameRepository
    {
        public IEnumerable<SearchResult> SearchGames(string searchText)
        {
            using (IDatabase db = DBFactory.GetDatabase())
            {
                var results = db.Query<SearchResult>("SELECT ID AS `Value`, Name AS Label, CoverImagePath FROM tbl_Game WHERE Name LIKE CONCAT('%', @0, '%') LIMIT 10;", searchText).ToList();

                return results;
            }
        }                
    }
}

