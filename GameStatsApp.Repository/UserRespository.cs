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

        public IEnumerable<IDNamePair> GetDefaultLists()
        {
            using (IDatabase db = DBFactory.GetDatabase())
            {
                return db.Query<IDNamePair>("SELECT ID, Name FROM tbl_DefaultList;").ToList();
            }
        }

        public IEnumerable<UserList> GetUserLists(Expression<Func<UserList, bool>> predicate)
        {
            using (IDatabase db = DBFactory.GetDatabase())
            {
                return db.Query<UserList>().Where(predicate).Where(i => !i.Deleted).ToList();
            }
        }

        public IEnumerable<UserListView> GetUserListViews(Expression<Func<UserListView, bool>> predicate)
        {
            using (IDatabase db = DBFactory.GetDatabase())
            {
                return db.Query<UserListView>().Where(predicate).ToList();
            }
        }

        public IEnumerable<UserListGameView> GetUserListGameViews(Expression<Func<UserListGameView, bool>> predicate)
        {
            using (IDatabase db = DBFactory.GetDatabase())
            {
                return db.Query<UserListGameView>().Where(predicate).ToList();
            }
        }       

        public IEnumerable<UserListGameView> GetUserListGames(int userID, int userListID)
        {
            using (IDatabase db = DBFactory.GetDatabase())
            {
                var results = db.Query<UserListGameView>("CALL GetUserListGames (@0, @1);", userID, userListID).ToList();

                return results;
            }
        }          

        public void SaveUserLists(IEnumerable<UserList> userLists)
        {
            using (IDatabase db = DBFactory.GetDatabase())
            {
                using (var tran = db.GetTransaction())
                {
                    foreach (var userList in userLists)
                    {
                        db.Save<UserList>(userList);
                    }

                    tran.Complete();
                }
            }
        }

        public void SaveUserList(UserList userList)
        {
            using (IDatabase db = DBFactory.GetDatabase())
            {
                db.Save<UserList>(userList);
            }
        }
        
        public void SaveUserListGame(UserListGame userListGame)
        {
            using (IDatabase db = DBFactory.GetDatabase())
            {
                db.Save<UserListGame>(userListGame);
            }
        }

        public void SaveUserListGames(IEnumerable<UserListGame> userListGames)
        {
            using (IDatabase db = DBFactory.GetDatabase())
            {
                using (var tran = db.GetTransaction())
                {
                    foreach (var userListGame in userListGames)
                    {
                        db.Save<UserListGame>(userListGame);
                    }

                    tran.Complete();
                }
            }
        }              

        public void DeleteUserListGame(int userListID, int gameID)
        {
            using (IDatabase db = DBFactory.GetDatabase())
            {
                db.DeleteWhere<UserListGame>("UserListID = @0 AND GameID = @1", userListID, gameID);
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

