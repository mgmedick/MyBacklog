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
        IEnumerable<SearchResult> SearchUsers(string searchText);
    }
}






