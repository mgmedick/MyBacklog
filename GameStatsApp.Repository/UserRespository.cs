using System;
using System.Collections.Generic;
using NPoco;
using Serilog;
using NPoco.Extensions;
using System.Linq;
using GameStatsApp.Model;
using GameStatsApp.Model.Data;
using GameStatsApp.Interfaces.Repositories;
using Microsoft.Extensions.Configuration;
using System.Linq.Expressions;
using System.Collections;

namespace GameStatsApp.Repository
{
    public class UserRespository : BaseRepository, IUserRepository
    {
        public IEnumerable<User> GetUsers(Expression<Func<User, bool>> predicate)
        {
            using (IDatabase db = DBFactory.GetDatabase())
            {
                return db.Query<User>().Where(predicate).ToList();
            }
        }

        public void SaveUser(User userAcct)
        {
            using (IDatabase db = DBFactory.GetDatabase())
            {
                using (var tran = db.GetTransaction())
                {
                    db.Save<User>(userAcct);

                    tran.Complete();
                }
            }
        }

        public IEnumerable<UserView> GetUserViews(Expression<Func<UserView, bool>> predicate)
        {
            using (IDatabase db = DBFactory.GetDatabase())
            {
                return db.Query<UserView>().Where(predicate).ToList();
            }
        }

        public void SaveUserSetting(UserSetting userAcctSetting)
        {
            using (IDatabase db = DBFactory.GetDatabase())
            {
                using (var tran = db.GetTransaction())
                {
                    db.Save<UserSetting>(userAcctSetting);

                    tran.Complete();
                }
            }
        }
    }
}

