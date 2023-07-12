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

        public IEnumerable<UserGameAccount> GetUserGameAccounts(Expression<Func<UserGameAccount, bool>>  predicate)
        {
            using (IDatabase db = DBFactory.GetDatabase())
            {
                return db.Query<UserGameAccount>().Where(predicate).ToList();
            }
        }
        
        public void SaveUserGameAccount(UserGameAccount userGameAccount)
        {
            using (IDatabase db = DBFactory.GetDatabase())
            {
                using (var tran = db.GetTransaction())
                {
                    db.Save<UserGameAccount>(userGameAccount);

                    tran.Complete();
                }
            }
        }

        public IEnumerable<IDNamePair> GetDefaultGameLists()
        {
            using (IDatabase db = DBFactory.GetDatabase())
            {
                return db.Query<IDNamePair>("SELECT ID, Name FROM tbl_DefaultGameList;").ToList();
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

        public IEnumerable<GameView> GetGamesByUserGameList(int userGameListID)
        {
            using (IDatabase db = DBFactory.GetDatabase())
            {
                var results = db.Query<GameView>("CALL GetGamesByUserGameList (@0);", userGameListID).ToList();

                return results;
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

        public void SaveUserGameListGame(UserGameListGame userGameListGame)
        {
            using (IDatabase db = DBFactory.GetDatabase())
            {
                db.Save<UserGameListGame>(userGameListGame);
            }
        }

        public void SaveUserGameListGames(IEnumerable<UserGameListGame> userGameListGames)
        {
            using (IDatabase db = DBFactory.GetDatabase())
            {
                using (var tran = db.GetTransaction())
                {
                    foreach (var userGameListGame in userGameListGames)
                    {
                        db.Save<UserGameListGame>(userGameListGame);
                    }

                    tran.Complete();
                }
            }
        }              

        public void DeleteUserGameListGame(int userGameListID, int gameID)
        {
            using (IDatabase db = DBFactory.GetDatabase())
            {
                db.DeleteWhere<UserGameListGame>("UserGameListID = @0 AND GameID = @1", userGameListID, gameID);
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

