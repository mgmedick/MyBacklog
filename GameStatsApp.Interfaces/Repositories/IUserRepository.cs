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
        IEnumerable<UserAccount> GetUserAccounts(Expression<Func<UserAccount, bool>>  predicate);
        IEnumerable<UserAccountView> GetUserAccountViews(Expression<Func<UserAccountView, bool>>  predicate);
        void SaveUserAccount(UserAccount userAccount);
        void SaveUserAccountTokens(int userAccountID, IEnumerable<UserAccountToken> userAccountTokens);
        IEnumerable<IDNamePair> GetDefaultLists();
        IEnumerable<UserList> GetUserLists(Expression<Func<UserList, bool>> predicate);
        IEnumerable<UserListView> GetUserListViews(Expression<Func<UserListView, bool>> predicate);
        void SaveUserLists(IEnumerable<UserList> userLists);
        void SaveUserList(UserList userList);
        void SaveUserListGame(UserListGame userListGame);
        void SaveUserListGames(IEnumerable<UserListGame> userListGames);
        void DeleteUserListGame(int userListID, int gameID);
        IEnumerable<UserListGameView> GetUserListGameViews(Expression<Func<UserListGameView, bool>> predicate);
        IEnumerable<UserListGameView> GetUserListGames(int userListID);
        IEnumerable<SearchResult> SearchUsers(string searchText);
        AboutResult GetAbout();
    }
}






