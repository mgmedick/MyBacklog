using System;
using GameStatsApp.Model;
using GameStatsApp.Model.JSON;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameStatsApp.Interfaces.Services
{
    public interface IGameService
    {
        IEnumerable<SearchResult> SearchGames(string searchText);
        Task<List<GameNameResult>> GetSteamUserGameNames(string steamID);
        Task<List<GameNameResult>> GetMicrosoftUserGameNames(string userHash, string xstsToken, ulong userXuid);
    }
}
