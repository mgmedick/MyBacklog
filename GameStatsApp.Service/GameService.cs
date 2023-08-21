using GameStatsApp.Interfaces.Repositories;
using GameStatsApp.Interfaces.Services;
using GameStatsApp.Model;
using GameStatsApp.Model.Data;
using GameStatsApp.Model.ViewModels;
//using GameStatsApp.Interfaces.Helpers;
using System.Collections.Generic;
using System.Linq;

namespace GameStatsApp.Service
{
    public class GameService : IGameService
    {
        private readonly IGameRepository _gameRepo = null;

        public GameService(IGameRepository gameRepo)
        {
            _gameRepo = gameRepo;
        }

        public IEnumerable<SearchResult> SearchGames(string searchText)
        {
            var results = new List<SearchResult>();

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                searchText = searchText.Trim();
                results = _gameRepo.SearchGames(searchText).ToList();
            }
            
            return results;
        }
    }
}
