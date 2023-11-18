using System;
using System.Collections.Generic;
using GameStatsApp.Model;
using GameStatsApp.Model.Data;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace GameStatsApp.Interfaces.Repositories
{
    public interface IGameRepository
    {
        IEnumerable<Game> GetGames(Expression<Func<Game, bool>>  predicate = null);
        IEnumerable<SearchResult> SearchGames(string searchText);
    }
}






