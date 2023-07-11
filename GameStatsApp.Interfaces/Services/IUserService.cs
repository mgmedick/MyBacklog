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
        void CreateUser(string email, string username, string pass);
        IEnumerable<UserView> GetUserViews(Expression<Func<UserView, bool>> predicate);        
        Task SendResetPasswordEmail(string email);
        ChangePasswordViewModel GetChangePassword(string email, long expirationTime, string token);
        void ChangeUserPassword(string email, string pass);
        void ChangeUsername(string email, string username);
        void AddGameToUserGameList(int userID, int userGameListID, int gameID);
        void RemoveGameFromUserGameList(int userID, int userGameListID, int gameID);
        void RemoveGameFromAllUserGameLists(int userID, int gameID);
        bool EmailExists(string email);
        bool PasswordMatches(string password, string email);
        bool UsernameExists(string username, bool activeFilter);
        bool EmailExists(string email, bool activeFilter);
        Task SendConfirmRegistrationEmail(string email, string username);
        IEnumerable<SearchResult> SearchUsers(string searchText);
        IEnumerable<UserGameList> GetUserGameLists (int userID);
        IEnumerable<GameViewModel> GetGamesByUserGameList (int userGameListID);
        void SaveUserGameAccount(int userID, int gameAccountTypeID, TokenResponse tokenResponse);
        IEnumerable<UserGameAccountViewModel> GetUserGameAccounts(int userID);   
        Task<Tuple<UserGameAccount, string>> GetAndReAuthUserGameAccount(int userID, int userGameAccountID);
        Task ImportGamesFromUserGameAccount(UserGameAccount userGameAccount);
    }
}
