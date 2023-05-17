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
        IEnumerable<User> GetUser(Expression<Func<User, bool>> predicate);
        void SaveUser(User userAcct);
        IEnumerable<UserView> GetUserViews(Expression<Func<UserView, bool>> predicate);
        void SaveUserSetting(UserSetting userAcctSetting);
    }
}






