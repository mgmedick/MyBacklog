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
        Task<JObject> RefreshAccessToken(string refreshToken);
        Task<JObject> ExchangeCodeForAccessToken(string code, string redirectUri);
        Task<JObject> ExchangeRpsTicketForUserToken(string rpsTicket);
        Task<JObject> ExchangeTokenForXSTSToken(string userToken);
        Task<JObject> GetUserInventory(string userHash, string xstsToken, ulong userXuid);
        Task<JArray> GetUserTitleHistory(string userHash, string xstsToken, ulong userXuid, JArray results = null, string continuationToken = null);
        // string GetRefreshWindowsLiveAuthUrl(AccessToken refreshToken);
        // void SetAccessToken(string url);
        // WindowsLiveResponse ParseWindowsLiveResponse(string url);
        // Task<bool> AuthenticateAsync();
        // Task<WindowsLiveResponse> RefreshLiveTokenAsync(AccessToken refreshToken);
        // Task<AccessToken> AuthenticateXASUAsync(AccessToken accessToken);
        // Task<Tuple<AccessToken, XboxUserInformation>> AuthenticateXSTSAsync(AccessToken userToken);
    }
}
