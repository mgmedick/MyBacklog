using System;
using GameStatsApp.Model.ViewModels;
using GameStatsApp.Model;
using GameStatsApp.Model.Data;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using GameStatsApp.Model.JSON;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GameStatsApp.Interfaces.Services
{
    public interface IAuthService
    {
        string GetSteamAuthUrl(string redirectUri);
        Task<List<string>> GetSteamUserGameNames(string steamID);
        Task<TokenResponse> AuthenticateSteam(Dictionary<string, string> results);
        string GetMicrosoftAuthUrl(string redirectUri);
        Task<TokenResponse> AuthenticateMicrosoft(string code, string redirectUri);
        Task<TokenResponse> ReAuthenticateMicrosoft(string refreshToken);
        Task<List<string>> GetMicrosoftUserGameNames(string userHash, string xstsToken, ulong userXuid, DateTime? importLastRunDate = null);
        Task<SocialTokenResponse> ValidateSocialToken(string accessToken, int socialAccountTypeID);
        Task<bool> ValidateGoogleRecaptcha(string token);
    }
}
