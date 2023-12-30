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

    public enum AccountType
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

    public enum DefaultList
    {
        Backlog = 1,
        Playing = 2,
        Completed = 3 
    }

    public enum ImportType
    {
        File = 1,
        Steam = 2,
        Xbox = 3
    }
    
    public enum GameCategory
    {
        Main = 0,
        DLC = 1,
        Expansion = 2,
        Bundle = 3,
        StandaloneExpansion = 4,
        Mod = 5,
        Episode = 6,
        Season = 7,
        Remake = 9,
        Expanded = 10,
        Port = 11,
        Fork = 12,
        Pack = 13,
        Update = 14
    }      
}
