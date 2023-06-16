using System;
using GameStatsApp.Model.ViewModels;
using GameStatsApp.Model;
using GameStatsApp.Model.Data;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using GameStatsApp.Model.JSON;

namespace GameStatsApp.Interfaces.Services
{
    public interface IAuthService
    {
        string GetWindowsLiveAuthUrl();
        void Authenticate(string code);
        Task<dynamic> ExchangeCodeForAccessToken(string code);
        Task<dynamic> ExchangeRpsTicketForUserToken(string rpsTicket);
        // string GetRefreshWindowsLiveAuthUrl(AccessToken refreshToken);
        // void SetAccessToken(string url);
        // WindowsLiveResponse ParseWindowsLiveResponse(string url);
        // Task<bool> AuthenticateAsync();
        // Task<WindowsLiveResponse> RefreshLiveTokenAsync(AccessToken refreshToken);
        // Task<AccessToken> AuthenticateXASUAsync(AccessToken accessToken);
        // Task<Tuple<AccessToken, XboxUserInformation>> AuthenticateXSTSAsync(AccessToken userToken);
    }
}
