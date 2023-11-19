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
        public IEnumerable<User> GetUsers(Expression<Func<User, bool>>  predicate = null)
        {
            using (IDatabase db = DBFactory.GetDatabase())
            {
                return db.Query<User>().Where(predicate ?? (x => true)).ToList();
            }
        }

        public void SaveUser(User user)
        {
            using (IDatabase db = DBFactory.GetDatabase())
            {
                using (var tran = db.GetTransaction())
                {
                    db.Save<User>(user);

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

        public void SaveUserSetting(UserSetting userSetting)
        {
            using (IDatabase db = DBFactory.GetDatabase())
            {
                using (var tran = db.GetTransaction())
                {
                    db.Save<UserSetting>(userSetting);

                    tran.Complete();
                }
            }
        }

        public IEnumerable<UserAccount> GetUserAccounts(Expression<Func<UserAccount, bool>>  predicate)
        {
            using (IDatabase db = DBFactory.GetDatabase())
            {
                return db.Query<UserAccount>().Where(predicate).ToList();
            }
        }
        
        public IEnumerable<UserAccountView> GetUserAccountViews(Expression<Func<UserAccountView, bool>>  predicate)
        {
            using (IDatabase db = DBFactory.GetDatabase())
            {
                return db.Query<UserAccountView>().Where(predicate).ToList();
            }
        }

        public void SaveUserAccount(UserAccount userAccount)
        {
            using (IDatabase db = DBFactory.GetDatabase())
            {
                using (var tran = db.GetTransaction())
                {
                    db.Save<UserAccount>(userAccount);
                    tran.Complete();
                }
            }            
        }

        public void SaveUserAccountTokens(int userAccountID, IEnumerable<UserAccountToken> userAccountTokens)
        {
            using (IDatabase db = DBFactory.GetDatabase())
            {
                using (var tran = db.GetTransaction())
                {
                    db.DeleteWhere<UserAccountToken>("UserAccountID = @0", userAccountID);
                    foreach(var userAccountToken in userAccountTokens)
                    {
                        userAccountToken.UserAccountID = userAccountID;
                        db.Save<UserAccountToken>(userAccountToken);
                    }

                    tran.Complete();
                }
            }
        }

        public void SaveUserAccount(UserAccount userAccount, IEnumerable<UserAccountToken> userAccountTokens)
        {
            using (IDatabase db = DBFactory.GetDatabase())
            {
                using (var tran = db.GetTransaction())
                {
                    db.Save<UserAccount>(userAccount);

                    foreach(var userAccountToken in userAccountTokens)
                    {
                        userAccountToken.UserAccountID = userAccount.ID;
                        db.Save<UserAccountToken>(userAccountToken);
                    }

                    tran.Complete();
                }
            }
        }
                                            
        public IEnumerable<SearchResult> SearchUsers(string searchText)
        {
            using (IDatabase db = DBFactory.GetDatabase())
            {
                var results = db.Query<SearchResult>("SELECT ID AS `Value`, Username AS Label FROM tbl_User WHERE Username LIKE CONCAT('%', @0, '%') LIMIT 10;", searchText).ToList();

                return results;
            }
        }                     
    }
}

