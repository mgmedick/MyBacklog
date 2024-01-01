using GameStatsApp.Interfaces.Repositories;
using GameStatsApp.Interfaces.Services;
using GameStatsApp.Model;
using GameStatsApp.Common.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System;
using GameStatsApp.Model.Data;
using GameStatsApp.Model.JSON;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.ComponentModel.DataAnnotations;

namespace GameStatsApp.Service
{
    public class GameService : IGameService
    {
        private readonly IAuthService _authService = null; 
        private readonly ICacheService _cacheService = null;

        public GameService(IAuthService authService, ICacheService cacheService)
        {
            _authService = authService; 
            _cacheService = cacheService; 
        }

        /*
        public IEnumerable<SearchResult> SearchGames(string searchText)
        {
            var results = new List<SearchResult>();

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                searchText = searchText.SanatizeGameName();
                results = _cacheService.GetGameViews()
                               .Where(i => i.SantizedName.Contains(searchText, StringComparison.OrdinalIgnoreCase))
                               .GroupBy(g => new { g.Name, g.ReleaseDate?.Year })
                               .Select(i => i.First())
                               .Select(i => new { i.ID, i.Name, i.ReleaseDate, i.CoverImagePath, Priority = i.SantizedName.Length - searchText.Length })
                               .OrderBy(i => i.Priority)
                               .ThenByDescending(i => i.ReleaseDate)
                               .ThenBy(i => i.Name)
                               .Select(i => new SearchResult() { Value = i.ID.ToString(), Label = i.Name, LabelSecondary = i.ReleaseDate?.Year.ToString(), ImagePath = i.CoverImagePath })
                               .Take(20)
                               .ToList();
            }
            
            return results;
        }
        */

        public IEnumerable<SearchResult> SearchGames(string searchText)
        {
            var results = new List<SearchResult>();

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                searchText = searchText.SanatizeGameName();
                var searchItems = searchText.Split(' ');
                results = _cacheService.GetGameViews()
                               .Where(i => searchItems.Any(g => i.SantizedName.Contains(g, StringComparison.OrdinalIgnoreCase)))
                               .GroupBy(g => new { g.Name, g.ReleaseDate?.Year })
                               .Select(i => i.First())
                               .Select(i => new { i.ID, i.Name, i.ReleaseDate, i.CoverImagePath, 
                                    ContainsPriority = searchItems.Count(g => i.SantizedName.Contains(g, StringComparison.OrdinalIgnoreCase)),
                                    MatchPriority = searchItems.Intersect(i.SantizedName.Split(' '), StringComparer.OrdinalIgnoreCase).Count(),
                                    RemainderPriority = i.SantizedName.Replace(searchItems, string.Empty, StringComparison.OrdinalIgnoreCase).Length
                               })
                               .OrderByDescending(i => i.ContainsPriority)
                               .ThenByDescending(i => i.MatchPriority)
                               .ThenBy(i => i.RemainderPriority)   
                               .ThenByDescending(i => i.ReleaseDate)
                               .Select(i => new SearchResult() { Value = i.ID.ToString(), Label = i.Name, LabelSecondary = i.ReleaseDate?.Year.ToString(), ImagePath = i.CoverImagePath })
                               .Take(20)
                               .ToList();
            }
            
            return results;
        }

        public async Task<List<GameNameResult>> GetSteamUserGameNames(string steamID)
        {
            var results = new List<GameNameResult>();
            var items = await _authService.GetSteamUserInventory(steamID);
            results = items.Reverse()
                           .Select((obj, index) => new GameNameResult() { Name = (string)obj["name"],
                                                                    SantizedName = ((string)obj["name"]).SanatizeGameName(),
                                                                    SortOrder = index
                                                                  })
                            .ToList();

            results = results.GroupBy(g => new { g.Name })
                .Select(i => i.First())
                .ToList();
                                          
            return results;
        }

        public async Task<List<GameNameResult>> GetMicrosoftUserGameNames(string userHash, string xstsToken, ulong userXuid)
        {
            var results = new List<GameNameResult>();
            var items = await _authService.GetMicrosoftUserTitleHistory(userHash, xstsToken, userXuid);
            results = items.Where(obj => ((string)obj["type"]) == "Game")
                           .Reverse()
                           .Select((obj, index) => new GameNameResult() { Name = (string)obj["name"],
                                                                    SantizedName = ((string)obj["name"]).SanatizeGameName(),
                                                                    SortOrder = index                                                                    
                                                                  })
                            .ToList();

            results = results.GroupBy(g => new { g.Name })
                .Select(i => i.First())
                .ToList();
                                      
            return results;
        }          
    }
}
