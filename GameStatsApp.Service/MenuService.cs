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
    public class MenuService : IMenuService
    {
        private readonly IUserRepository _userRepo = null;

        public MenuService(IUserRepository userRepo)
        {
            _userRepo = userRepo;
        }

        public IEnumerable<SearchResult> Search(string searchText)
        {
            var results = new List<SearchResult>();

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                searchText = searchText.Trim();
                var users = _userRepo.SearchUsers(searchText).ToList();
                if (users.Any()){
                    var usersGroup = new SearchResult { Value = "0", Label = "Users", SubItems = users };
                    results.Add(usersGroup);
                }
            }

            return results;
        }

        public AboutViewModel GetAbout()
        {
            var item = _userRepo.GetAbout();
            var result = new AboutViewModel() { UserCount = item.UserCount, GameName = item.GameName, UserListName = item.UserListName };

            return result;
        }          
    }
}
