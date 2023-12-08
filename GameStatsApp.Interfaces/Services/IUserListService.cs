using System;
using GameStatsApp.Model.ViewModels;
using GameStatsApp.Model;
using GameStatsApp.Model.Data;
using GameStatsApp.Model.JSON;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace GameStatsApp.Interfaces.Services
{
    public interface IUserListService
    {
        UserListGameViewModel AddNewGameToUserList(int userID, int userListID, int gameID);
        void AddGameToUserList(int userID, int userListID, int gameID);
        void RemoveGameFromUserList(int userID, int userListID, int gameID);
        void RemoveGameFromAllUserLists(int userID, int gameID);
        void RemoveAllGamesFromUserList(int userID, int userListID);
        IEnumerable<UserListViewModel> GetUserLists (int userID);
        void SaveUserList(int userID, UserListViewModel userListVM);
        void DeleteUserList(int userID, int userListID);
        void UpdateUserListSortOrders(int userID, List<int> userListIDs);
        void UpdateUserListActive(int userID, int userListID, bool active);
        IEnumerable<UserListGameViewModel> GetUserListGames (int userID, int userListID);     
        Task<int> ImportGamesFromUserAccount(int userListID, UserAccountView userAccountVW);   
        bool UserListNameExists(int userID, int userListID, string userListName);
    }
}
