using System;
using GameStatsApp.Model;
using GameStatsApp.Model.Data;
using GameStatsApp.Model.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameStatsApp.Interfaces.Services
{
    public interface IGameService
    {
        IEnumerable<SearchResult> SearchGames(string searchText);
        Task<int> ImportGames(int userID, UserAccountView userAccountVW);
        string GetSanatizedGameName(string gameName);
    }
}
