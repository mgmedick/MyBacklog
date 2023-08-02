using System.Runtime.Serialization;
using System.ComponentModel;

namespace GameStatsApp.Model
{
    public enum Template
    {
        ActivateEmail = 1,
        ResetPasswordEmail = 2,
        ConfirmRegistration = 3
    }       

    public enum GameAccountType
    {
        Steam = 1,
        Xbox = 2
    }

    public enum SocialAccountType
    {
        Google = 1,
        Facebook = 2
    }    

    public enum TokenType
    {
        Access = 1,
        Refresh = 2
    }      

    public enum DefaultGameList
    {
        AllGames = 1,
        Backlog = 2,
        Playing = 3,
        Completed = 4
    } 
}
