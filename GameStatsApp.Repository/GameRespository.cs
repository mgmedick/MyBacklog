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
        public IEnumerable<Game> GetGames(Expression<Func<Game, bool>>  predicate)
        {
            using (IDatabase db = DBFactory.GetDatabase())
            {
                return db.Query<Game>().Where(predicate).ToList();
            }
        }

        public IEnumerable<SearchResult> SearchGames(string searchText)
        {
            using (IDatabase db = DBFactory.GetDatabase())
            {
                var results = db.Query<SearchResult>("SELECT ID AS Value, Name AS Label, YEAR(ReleaseDate) AS LabelSecondary, CoverImagePath AS ImagePath FROM vw_Game WHERE Name LIKE CONCAT('%', @0, '%') ORDER BY ReleaseDate LIMIT 10;", searchText).ToList();

                return results;
            }
        }                
    }
}

