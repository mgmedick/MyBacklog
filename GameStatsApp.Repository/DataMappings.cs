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
            For<UserGameService>().PrimaryKey("ID", false).TableName("tbl_User_GameService");
            For<UserGameList>().PrimaryKey("ID", false).TableName("tbl_User_GameList");
            For<UserView>().TableName("vw_User");
            For<Setting>().PrimaryKey("ID").TableName("tbl_Setting");
        }
    }
}



