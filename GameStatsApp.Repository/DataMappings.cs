using System;
using System.Collections.Generic;
using System.Text;
using NPoco.FluentMappings;
using GameStatsApp.Model.Data;

namespace GameStatsApp.Repository
{
    public class DataMappings : Mappings
    {
        public DataMappings()
        {
            For<User>().PrimaryKey("ID").TableName("tbl_User");
            For<UserSetting>().PrimaryKey("UserID", false).TableName("tbl_User_Setting");
            For<UserGameAccount>().PrimaryKey("ID", false).TableName("tbl_UserGameAccount");
            For<UserGameAccountToken>().PrimaryKey("ID", false).TableName("tbl_UserGameAccount_Token");
            For<UserGameAccountDetail>().PrimaryKey("ID", false).TableName("tbl_UserGameAccount_Detail");          
            For<UserGameList>().PrimaryKey("ID", false).TableName("tbl_UserGameList");
            For<UserGameListView>().PrimaryKey("ID", false).TableName("vw_UserGameList");
            For<UserGameListGame>().PrimaryKey("ID", false).TableName("tbl_UserGameList_Game");
            For<UserView>().TableName("vw_User");
            For<Game>().TableName("tbl_Game");
            For<GameView>().TableName("vw_Game");
            For<Setting>().PrimaryKey("ID").TableName("tbl_Setting");
        }
    }
}



