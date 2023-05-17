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
        void CreateUser(string username, string pass);
        IEnumerable<User> GetUsers(Expression<Func<User, bool>> predicate);
        IEnumerable<UserView> GetUserViews(Expression<Func<UserView, bool>> predicate);        
        Task SendResetPasswordEmail(string username);
        ChangePasswordViewModel GetChangePassword(string username, string email, long expirationTime, string token);
        void ChangeUserAcctPassword(string username, string pass);
        bool EmailExists(string email);
        bool PasswordMatches(string password, string username);
        bool UsernameExists(string username, bool activeFilter);
        UserViewModel GetUser(int userID);
        void SaveUser(UserViewModel userAcctVM, int currUserAcctID);
        void UpdateIsDarkTheme(int currUserID, bool isDarkTheme);
        Task SendConfirmRegistrationEmail(string email, string username);
    }
}
