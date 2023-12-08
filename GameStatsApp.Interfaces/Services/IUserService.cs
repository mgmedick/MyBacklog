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
        bool EmailExists(string email);
        bool PasswordMatches(string password, string email);
        bool UsernameExists(string username, bool activeFilter);
        bool EmailExists(string email, bool activeFilter);
        Task SendConfirmRegistrationEmail(string email, string username);            
        void SaveUserAccount(int userID, int accountTypeID, TokenResponse tokenResponse);
        Task<UserAccountView> GetRefreshedUserAccount(int userID, int userAccountID);
    }
}
