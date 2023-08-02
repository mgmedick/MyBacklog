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
        string GetWindowsLiveAuthUrl(string redirectUri);
        Task<TokenResponse> Authenticate(string code, string redirectUri);
        Task<TokenResponse> ReAuthenticate(string refreshToken);
        Task<JObject> ExchangeCodeForAccessToken(string code, string redirectUri);
        Task<JObject> ExchangeRpsTicketForUserToken(string rpsTicket);
        Task<JObject> ExchangeTokenForXSTSToken(string userToken);
        Task<List<string>> GetUserGameNames(string userHash, string xstsToken, ulong userXuid, DateTime? importLastRunDate = null);
        Task<JArray> GetUserTitleHistory(string userHash, string xstsToken, ulong userXuid, JArray results = null, string continuationToken = null);
        Task<SocialTokenResponse> ValidateSocialToken(string accessToken, int socialAccountTypeID);
        Task<string> ValidateFacebookToken(string accessToken);
        Task<JObject> GetFacebookResponse(string accessToken);
    }
}
