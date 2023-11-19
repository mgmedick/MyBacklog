using System;
using System.Collections.Generic;
using GameStatsApp.Model;
using GameStatsApp.Model.Data;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace GameStatsApp.Interfaces.Repositories
{
    public interface IUserListRepository
    {
        IEnumerable<IDNamePair> GetDefaultLists();
        IEnumerable<UserList> GetUserLists(Expression<Func<UserList, bool>> predicate);
        IEnumerable<UserListView> GetUserListViews(Expression<Func<UserListView, bool>> predicate);
        void SaveUserLists(IEnumerable<UserList> userLists);
        void SaveUserList(UserList userList);
        void SaveUserListGame(UserListGame userListGame);
        void SaveUserListGames(IEnumerable<UserListGame> userListGames);
        void DeleteUserListGame(int userListID, int gameID);
        void DeleteAllUserListGames(int userListID);
        IEnumerable<UserListGameView> GetUserListGameViews(Expression<Func<UserListGameView, bool>> predicate);
        IEnumerable<UserListGameView> GetUserListGames(int userID, int userListID);
    }
}






