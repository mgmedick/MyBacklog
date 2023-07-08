﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace GameStatsApp.Model.Data
{
    public class UserView
    {
        public int UserID { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public bool PromptToChange { get; set; }
        public bool Active { get; set; }
        public bool Deleted { get; set; }
        public bool Locked { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsDarkTheme { get; set; }
        public string GameAccountTypeIDs { get; set; }

        public User ConvertToUser()
        {
            return new User {
                ID = UserID,
                Username = Username,
                Email = Email,
                Password = Password,
                PromptToChange = PromptToChange,
                Active = Active,
                Deleted = Deleted,
                Locked = Locked,
                CreatedBy = CreatedBy,
                CreatedDate = CreatedDate,
                ModifiedBy = ModifiedBy,
                ModifiedDate = ModifiedDate
            };
        }   
    }
} 
