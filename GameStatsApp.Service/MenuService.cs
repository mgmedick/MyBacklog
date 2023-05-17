using SpeedRunApp.Interfaces.Repositories;
using SpeedRunApp.Interfaces.Services;
using SpeedRunApp.Model;
using SpeedRunApp.Model.Data;
using SpeedRunApp.Model.ViewModels;
//using SpeedRunApp.Interfaces.Helpers;
using System.Collections.Generic;
using System.Linq;

namespace SpeedRunApp.Service
{
    public class MenuService : IMenuService
    {
        private readonly IUserService _userService = null;

        public MenuService(IUserService userService)
        {
            _userService = userService;
        }

        public IEnumerable<SearchResult> Search(string searchText)
        {
            var results = new List<SearchResult>();

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                searchText = searchText.Trim();
                var users = _userService.SearchUsers(searchText);
                if (users.Any()){
                    var usersGroup = new SearchResult { Value = "0", Label = "Users", SubItems = users };
                    results.Add(usersGroup);
                }
            }

            return results;
        }
    }
}
