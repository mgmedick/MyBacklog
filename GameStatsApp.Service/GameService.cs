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

namespace GameStatsApp.Service
{
    public class GameService : IGameService
    {
        private readonly IGameRepository _gameRepo = null;
        private readonly IAuthService _authService = null;
        private readonly IUserRepository _userRepo = null;
        private readonly IUserListRepository _userListRepo = null;
        private readonly ICacheService _cacheService = null;

        public GameService(IGameRepository gameRepo, IAuthService authService, IUserRepository userRepo, IUserListRepository userListRepo, ICacheService cacheService)
        {
            _gameRepo = gameRepo;
            _authService = authService;
            _userRepo = userRepo;
            _userListRepo = userListRepo;
            _cacheService = cacheService; 
        }

        public IEnumerable<SearchResult> SearchGames(string searchText)
        {
            var results = new List<SearchResult>();

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                searchText = searchText.SanatizeGameName();                                                           
                results = _cacheService.GetGameViews()
                                       .Where(i => i.SantizedName.Contains(searchText, StringComparison.OrdinalIgnoreCase) || i.SantizedNameNoSpace.Contains(searchText, StringComparison.OrdinalIgnoreCase))
                                       .OrderByDescending(i => i.ReleaseDate)
                                       .ThenBy(i => i.Name)
                                       .Select(i => new SearchResult() { Value = i.ID.ToString(), Label = i.Name, LabelSecondary = i.ReleaseDate?.Year.ToString(), ImagePath = i.CoverImagePath })
                                       .Take(20)
                                       .ToList();
            }
            
            return results;
        }

        public async Task<int> ImportGames(int userID, UserAccountView userAccountVW)
        {
            var importedGames = new List<GameImportResult>();

            switch (userAccountVW.AccountTypeID)
            {
                case (int)AccountType.Steam:
                    importedGames = await GetSteamUserGames(userAccountVW.AccountUserID);
                    break;           
                case (int)AccountType.Xbox:
                    importedGames = await GetMicrosoftUserGames(userAccountVW.AccountUserHash, userAccountVW.Token, Convert.ToUInt64(userAccountVW.AccountUserID));
                    break;
            }

            var games = _cacheService.GetGameViews().ToList();
            var foundGames = (from g in games
                              from gr in importedGames
                              where g.SantizedName.Equals(gr.SantizedName, StringComparison.OrdinalIgnoreCase)
                              orderby g.ID
                              select new { g.ID, gr.Name, gr.SantizedName, gr.SortOrder })
                            .GroupBy(i => i.SantizedName)
                            .Select(i => i.First())
                            .OrderBy(i => i.SortOrder)
                            .ToList();
            var missedGames = importedGames.Where(i => !foundGames.Any(x => x.SantizedName == i.SantizedName))
                                           .Select(i => i.Name)
                                           .ToList();

            var gameIDs = foundGames.Select(i => i.ID).Distinct().ToList();
            var existingGameIDs = _userListRepo.GetUserListGameViews(i=>i.UserListID == userAccountVW.UserListID).Select(i => i.ID).ToList();
            var userListGames = gameIDs.Where(i => !existingGameIDs.Contains(i))
                                           .Select(i => new UserListGame() { UserListID = userAccountVW.UserListID, GameID = i })
                                           .ToList();

            if (userListGames.Any())
            {
                _userListRepo.SaveUserListGames(userListGames);
            }

            userAccountVW.ImportLastRunDate = DateTime.UtcNow;
            _userRepo.SaveUserAccount(userAccountVW.ConvertToUserAccount());
            
            return userListGames.Count();
        } 

        public async Task<List<GameImportResult>> GetSteamUserGames(string steamID)
        {
            var results = new List<GameImportResult>();
            var items = await _authService.GetSteamUserInventory(steamID);
            results = items.Reverse()
                           .Select((obj, index) => new GameImportResult() { Name = (string)obj["name"],
                                                                    SantizedName = ((string)obj["name"]).SanatizeGameName(),
                                                                    SortOrder = index
                                                                  })
                            .ToList();

            results = results.GroupBy(g => new { g.Name })
                .Select(i => i.First())
                .ToList();
                                          
            return results;
        }

        public async Task<List<GameImportResult>> GetMicrosoftUserGames(string userHash, string xstsToken, ulong userXuid)
        {
            var results = new List<GameImportResult>();
            var items = await _authService.GetMicrosoftUserTitleHistory(userHash, xstsToken, userXuid);
            results = items.Where(obj => ((string)obj["type"]) == "Game")
                           .Reverse()
                           .Select((obj, index) => new GameImportResult() { Name = (string)obj["name"],
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
