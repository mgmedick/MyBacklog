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
    public interface IUserService
    {
        Task SendActivationEmail(string email);
        ActivateViewModel GetActivateUser(string email, long expirationTime, string token);
        int CreateDemoUser();
        int CreateUser(string email, string username, string pass);
        IEnumerable<UserView> GetUserViews(Expression<Func<UserView, bool>> predicate);        
        Task SendResetPasswordEmail(string email);
        ChangePasswordViewModel GetChangePassword(string email, long expirationTime, string token);
        void ChangeUserPassword(string email, string pass);
        void ChangeUsername(int userID, string username);
        void DeleteUser(int userID);
        UserListGameViewModel AddNewGameToUserList(int userID, int userListID, int gameID);
        void AddGameToUserList(int userID, int userListID, int gameID);
        void RemoveGameFromUserList(int userID, int userListID, int gameID);
        void RemoveGameFromAllUserLists(int userID, int gameID);
        void RemoveAllGamesFromUserList(int userID, int userListID);
        bool EmailExists(string email);
        bool PasswordMatches(string password, string email);
        bool UsernameExists(string username, bool activeFilter);
        bool EmailExists(string email, bool activeFilter);
        bool UserListNameExists(int userID, int userListID, string userListName);
        Task SendConfirmRegistrationEmail(string email, string username);
        IEnumerable<UserListViewModel> GetUserLists (int userID);
        void SaveUserList(int userID, UserListViewModel userListVM);
        void DeleteUserList(int userID, int userListID);
        void UpdateUserListSortOrders(int userID, List<int> userListIDs);
        void UpdateUserListActive(int userID, int userListID, bool active);
        IEnumerable<UserListGameViewModel> GetUserListGames (int userID, int userListID);               
        void SaveUserAccount(int userID, int accountTypeID, TokenResponse tokenResponse);
        IEnumerable<UserAccountViewModel> GetUserAccounts(int userID);   
        Task<UserAccountView> GetRefreshedUserAccount(int userID, int userAccountID);
        Task<int> ImportGames(int userID, UserAccountView userAccount);
    }
}
