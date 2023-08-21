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
                var games = _gameRepo.SearchGames(searchText).ToList();
                results = games.Select(i => new SearchResult() { 
                                                    Value = i.ID.ToString(), 
                                                    Label = "<span>" + i.Name + "</span>" + (i.ReleaseDate.HasValue ? "<br/><span class='text-muted'>" + i.ReleaseDate.Value.Year.ToString() + "</span>" : string.Empty),
                                                    ImagePath = i.CoverImagePath }).ToList();
            }

            return results;
        }
    }
}
