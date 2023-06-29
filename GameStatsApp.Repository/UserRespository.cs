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

        public void SaveUserGameService(UserGameService userGameService)
        {
            using (IDatabase db = DBFactory.GetDatabase())
            {
                using (var tran = db.GetTransaction())
                {
                    db.Save<UserGameService>(userGameService);

                    tran.Complete();
                }
            }
        }

        public IEnumerable<UserGameList> GetUserGameLists(Expression<Func<UserGameList, bool>> predicate)
        {
            using (IDatabase db = DBFactory.GetDatabase())
            {
                return db.Query<UserGameList>().Where(predicate).ToList();
            }
        }

        public IEnumerable<UserGameListView> GetUserGameListViews(Expression<Func<UserGameListView, bool>> predicate)
        {
            using (IDatabase db = DBFactory.GetDatabase())
            {
                return db.Query<UserGameListView>().Where(predicate).ToList();
            }
        }

        public void SaveUserGameLists(IEnumerable<UserGameList> userGameLists)
        {
            using (IDatabase db = DBFactory.GetDatabase())
            {
                using (var tran = db.GetTransaction())
                {
                    foreach (var userGameList in userGameLists)
                    {
                        db.Save<UserGameList>(userGameList);
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

