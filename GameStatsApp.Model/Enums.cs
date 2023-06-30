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

    public enum GameService
    {
        Steam = 1,
        Xbox = 2
    } 

    public enum DefaultGameList
    {
        AllGames = 1,
        Backlog = 2,
        Playing = 3,
        Completed = 4
    } 
}
