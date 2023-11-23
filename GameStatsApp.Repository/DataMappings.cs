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
            For<UserView>().TableName("vw_User");
            For<UserSetting>().PrimaryKey("UserID", false).TableName("tbl_User_Setting");
            For<UserAccount>().PrimaryKey("ID").TableName("tbl_UserAccount");
            For<UserAccountToken>().PrimaryKey("ID").TableName("tbl_UserAccount_Token");
            For<UserAccountView>().TableName("vw_UserAccount");
            For<UserList>().PrimaryKey("ID").TableName("tbl_UserList");
            For<UserListView>().TableName("vw_UserList");
            For<UserListGame>().PrimaryKey("ID").TableName("tbl_UserList_Game");
            For<UserListGameView>().TableName("vw_UserListGame");
            For<GameView>().TableName("vw_Game").Columns(i =>
            {
                i.Column(g => g.SantizedName).Ignore();
                i.Column(g => g.SantizedNameNoSpace).Ignore();
            });            
            For<Game>().PrimaryKey("ID").TableName("tbl_Game");
            For<Setting>().PrimaryKey("ID").TableName("tbl_Setting");
        }
    }
}



