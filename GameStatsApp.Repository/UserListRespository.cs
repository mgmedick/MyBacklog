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
    public class UserListRespository : BaseRepository, IUserListRepository
    {
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

        public void DeleteAllUserListGames(int userListID)
        {
            using (IDatabase db = DBFactory.GetDatabase())
            {
                db.DeleteWhere<UserListGame>("UserListID = @0", userListID);
            }
        }                                                                
    }
}

