using System;
using GameStatsApp.Model.ViewModels;
using GameStatsApp.Model;
using GameStatsApp.Model.Data;
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
        IEnumerable<User> GetUsers(Expression<Func<User, bool>> predicate);
        IEnumerable<UserView> GetUserViews(Expression<Func<UserView, bool>> predicate);        
        Task SendResetPasswordEmail(string email);
        ChangePasswordViewModel GetChangePassword(string email, long expirationTime, string token);
        void ChangeUserPassword(string email, string pass);
        void ChangeUsername(string email, string username);
        bool EmailExists(string email);
        bool PasswordMatches(string password, string email);
        bool UsernameExists(string username, bool activeFilter);
        bool EmailExists(string email, bool activeFilter);
        UserViewModel GetUser(int userID);
        void SaveUser(UserViewModel userVM, int currUserID);
        void UpdateIsDarkTheme(int currUserID, bool isDarkTheme);
        Task SendConfirmRegistrationEmail(string email, string username);
        IEnumerable<SearchResult> SearchUsers(string searchText);
    }
}
