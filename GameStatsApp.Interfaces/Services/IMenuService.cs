using GameStatsApp.Model;
using GameStatsApp.Model.Data;
using GameStatsApp.Model.ViewModels;
using System.Collections.Generic;

namespace GameStatsApp.Interfaces.Services
{
    public interface IMenuService
    {
        IEnumerable<SearchResult> Search(string searchText);
        AboutViewModel GetAbout();
    }
}
