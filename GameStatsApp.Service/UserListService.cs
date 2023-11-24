﻿using System;
using GameStatsApp.Interfaces.Repositories;
using GameStatsApp.Interfaces.Services;
using GameStatsApp.Model;
using GameStatsApp.Model.Data;
using GameStatsApp.Model.JSON;
using GameStatsApp.Model.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Expressions;
using GameStatsApp.Common.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace GameStatsApp.Service
{
    public class UserListService : IUserListService
    {
        private readonly IUserListRepository _userListRepo = null;
        public UserListService(IUserListRepository userListRepo)
        {
            _userListRepo = userListRepo;
        }

        public IEnumerable<UserListViewModel> GetUserLists (int userID)
        { 
            var userLists = _userListRepo.GetUserListViews(i => i.UserID == userID)
                                    .Select(i => new UserListViewModel(i))
                                    .OrderBy(i => i.SortOrder ?? i.ID)
                                    .ToList();

            return userLists;
        }     

        public void SaveUserList(int userID, UserListViewModel userListVM)
        {
            var userList = new UserList();
            var userLists = _userListRepo.GetUserLists(i=> i.UserID == userID).ToList();

            if (userListVM.ID == 0)
            {            
                userList = new UserList() { UserID = userID, 
                                            Name = userListVM.Name,
                                            Active = userListVM.Active,
                                            SortOrder = userLists.Count() + 1,
                                            CreatedDate = DateTime.UtcNow }; 
            }
            else
            {
                userList = userLists.FirstOrDefault(i=> i.ID == userListVM.ID);
                userList.Name = userListVM.Name;
                userList.Active = userListVM.Active;
                userList.SortOrder = userListVM.SortOrder;
                userList.ModifiedDate = DateTime.UtcNow;
            }

            if (userList.UserID == userID)
            {
                _userListRepo.SaveUserList(userList);
            }
        }    

        public void DeleteUserList(int userID, int userListID)
        {
            var userList = _userListRepo.GetUserLists(i => i.ID == userListID).FirstOrDefault();

            if (userList != null)
            {
                userList.Deleted = true;
                userList.ModifiedDate = DateTime.UtcNow;

                if (userList.UserID == userID)
                {
                    _userListRepo.SaveUserList(userList);
                }
            }
        }      

        public void UpdateUserListSortOrders(int userID, List<int> userListIDs)
        {
            var usersListIndexes = userListIDs.Select((i, index) => new { ID = i, index = index });
            var userLists = _userListRepo.GetUserLists(i => userListIDs.Contains(i.ID))                                    
                                     .ToList();
            userLists = (from c in userLists
                        join uc in usersListIndexes
                        on c.ID equals uc.ID
                        orderby uc.index
                        select c).ToList();

            var count = 0;
            foreach(var userList in userLists)
            {
                userList.SortOrder = count + 1;
                count++;
            }

            if (userLists.Count(i => i.UserID == userID) == userLists.Count())
            {
                _userListRepo.SaveUserLists(userLists);
            }          
        }      

        public void UpdateUserListActive(int userID, int userListID, bool active)
        {
            var userList = _userListRepo.GetUserLists(i => i.ID == userListID).FirstOrDefault();

            if (userList != null)
            {
                userList.Active = active;
                userList.ModifiedDate = DateTime.UtcNow;

                if (userList.UserID == userID)
                {
                    _userListRepo.SaveUserList(userList);
                }                    
            }        
        }                  

        public IEnumerable<UserListGameViewModel> GetUserListGames (int userID, int userListID)
        { 
            var gameVMs = _userListRepo.GetUserListGames(userID, userListID).Select(i => new UserListGameViewModel(i)).ToList();

            return gameVMs;
        }

        public UserListGameViewModel AddNewGameToUserList(int userID, int userListID, int gameID)
        {         
            AddGameToUserList(userID, userListID, gameID);

            return _userListRepo.GetUserListGameViews(i => i.UserListID == userListID && i.ID == gameID).Select(i => new UserListGameViewModel(i)).FirstOrDefault();
        }  

        public void AddGameToUserList(int userID, int userListID, int gameID)
        {         
            var gameIDs = _userListRepo.GetUserListGameViews(i => i.UserListID == userListID).Select(i => i.ID).ToList();

            if (!gameIDs.Contains(gameID))
            {
                var userListGame = new UserListGame() { UserListID = userListID, GameID = gameID };
                _userListRepo.SaveUserListGame(userListGame);
            }
        }        

        public void RemoveGameFromUserList(int userID, int userListID, int gameID)
        {         
            var gameIDs = _userListRepo.GetUserListGameViews(i => i.UserListID == userListID).Select(i => i.ID).ToList();

            if (gameIDs.Contains(gameID))
            {
                _userListRepo.DeleteUserListGame(userListID, gameID);
            }
        }

        public void RemoveGameFromAllUserLists(int userID, int gameID)
        {         
            var userListIDs = _userListRepo.GetUserListGameViews(i => i.UserID == userID && i.ID == gameID)
                                        .Select(i => i.UserListID)
                                        .Distinct()
                                        .ToList();

            foreach(var userListID in userListIDs)
            {
                _userListRepo.DeleteUserListGame(userListID, gameID);
            }
        }

        public void RemoveAllGamesFromUserList(int userID, int userListID)
        {         
            var userlistid = _userListRepo.GetUserLists(i => i.UserID == userID && i.ID == userListID)
                                        .Select(i => i.ID)
                                        .FirstOrDefault();

            if (userlistid > 0)
            {
                _userListRepo.DeleteAllUserListGames(userlistid);
            }
        }   

        public bool UserListNameExists(int userID, int userListID, string userListName)
        {
            var result = _userListRepo.GetUserLists(i => i.UserID == userID && i.Name == userListName && i.ID != userListID).Any();

            return result;
        }                   
    }
}