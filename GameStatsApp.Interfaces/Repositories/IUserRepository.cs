using System;
using System.Collections.Generic;
using GameStatsApp.Model;
using GameStatsApp.Model.Data;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace GameStatsApp.Interfaces.Repositories
{
    public interface IUserRepository
    {
        IEnumerable<User> GetUsers(Expression<Func<User, bool>> predicate = null);
        void SaveUser(User user);
        IEnumerable<UserView> GetUserViews(Expression<Func<UserView, bool>> predicate);
        void SaveUserSetting(UserSetting userSetting);
        void SaveUserGameService(UserGameService userGameService);
        IEnumerable<IDNamePair> GetDefaultGameLists();
        IEnumerable<UserGameList> GetUserGameLists(Expression<Func<UserGameList, bool>> predicate);
        IEnumerable<UserGameListView> GetUserGameListViews(Expression<Func<UserGameListView, bool>> predicate);
        void SaveUserGameLists(IEnumerable<UserGameList> userGameLists);
        void SaveUserGameListGame(UserGameListGame userGameListGame);
        void DeleteUserGameListGame(int userGameListID, int gameID);
        IEnumerable<SearchResult> SearchUsers(string searchText);
    }
}






