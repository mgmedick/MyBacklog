using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using GameStatsApp.Interfaces.Repositories;
using GameStatsApp.Interfaces.Services;
using GameStatsApp.Model;
using GameStatsApp.Model.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.WebUtilities;
using System.Collections.Specialized;
using GameStatsApp.Model.JSON;
using System.Text;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace GameStatsApp.Service
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _config = null;
        public string RpsTicket { get; set; }
        public string RefreshToken { get; set; }
        public string UserToken { get; set; }
        public XSTSTokenResponse XSTSTResponse { get; set; }
        public AuthService(IConfiguration config)
        {
            _config = config;
        }

        #region MicrosoftAuth
        public string GetWindowsLiveAuthUrl(string redirectUri)
        {
            var baseUrl = "https://login.live.com/oauth20_authorize.srf";
            var clientID = _config.GetSection("Auth").GetSection("Microsoft").GetSection("ClientId").Value;
            
            var parameters = new Dictionary<string,string>{
                {"client_id", clientID},
                {"scope", "XboxLive.signin XboxLive.offline_access"},
                {"response_type", "code"},
                {"redirect_uri", redirectUri},
            };

            var authUrl = QueryHelpers.AddQueryString(baseUrl, parameters);
            
            return authUrl;
        }

        public async Task<TokenResponse> Authenticate(string code, string redirectUri)
        {
            var accessResponse = await ExchangeCodeForAccessToken(code, redirectUri);
            var result = await GetTokenResponse(accessResponse);

            return result;
        }

        public async Task<TokenResponse> ReAuthenticate(string refreshToken)
        {
            var accessResponse = await RefreshAccessToken(refreshToken);
            var result = await GetTokenResponse(accessResponse);

            return result;
        }

        public async Task<TokenResponse> GetTokenResponse(JObject accessResponse)
        {
            RpsTicket = (string)accessResponse.GetValue("access_token");
            RefreshToken = (string)accessResponse.GetValue("refresh_token");

            var userResponse = await ExchangeRpsTicketForUserToken(RpsTicket);
            UserToken = (string)userResponse.GetValue("Token");

            var xstsResponse = await ExchangeTokenForXSTSToken(UserToken);
            XSTSTResponse = new XSTSTokenResponse() { Token = (string)xstsResponse.GetValue("Token"),
                                                      IssueInstant = (DateTime)xstsResponse.GetValue("IssueInstant"),
                                                      NotAfter = (DateTime)xstsResponse.GetValue("NotAfter"),
                                                      UserInformation = ((JObject)xstsResponse["DisplayClaims"]["xui"][0]).ToObject<XboxUserInformation>() };

            var tokenResponse = new TokenResponse() { Token = XSTSTResponse.Token,
                                                          IssuedDate = XSTSTResponse.IssueInstant,
                                                          ExpireDate = XSTSTResponse.NotAfter,
                                                          RefreshToken = RefreshToken,
                                                          AccountUserID = XSTSTResponse.UserInformation.XboxUserId.ToString(),
                                                          AccountUserHash = XSTSTResponse.UserInformation.Userhash
                                                    };

            return tokenResponse;
        }

        public async Task<JObject> ExchangeCodeForAccessToken(string code, string redirectUri)
        {
            JObject data = null;
            var clientID = _config.GetSection("Auth").GetSection("Microsoft").GetSection("ClientId").Value;
            var clientSecret = _config.GetSection("Auth").GetSection("Microsoft").GetSection("ClientSecret").Value;

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var parameters = new Dictionary<string,string>{
                    {"code", code},
                    {"client_id", clientID},
                    {"grant_type", "authorization_code"},
                    {"redirect_uri", redirectUri},
                    {"scope", "XboxLive.signin XboxLive.offline_access"},
                    {"client_secret", clientSecret}
                };

                var request = new HttpRequestMessage(HttpMethod.Post, "https://login.live.com/oauth20_token.srf") { Content = new FormUrlEncodedContent(parameters) };

                using (var response = await client.SendAsync(request))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var dataString = await response.Content.ReadAsStringAsync();
                        data = JObject.Parse(dataString);
                    }
                }
            }

            return data;
        }

        public async Task<JObject> RefreshAccessToken(string refreshToken)
        {
            JObject data = null;
            var clientID = _config.GetSection("Auth").GetSection("Microsoft").GetSection("ClientId").Value;
            var clientSecret = _config.GetSection("Auth").GetSection("Microsoft").GetSection("ClientSecret").Value;

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var parameters = new Dictionary<string,string>{
                    {"client_id", clientID},
                    {"grant_type", "refresh_token"},
                    {"scope", "XboxLive.signin XboxLive.offline_access"},
                    {"refresh_token", refreshToken},
                    {"client_secret", clientSecret}                 
                };

                var request = new HttpRequestMessage(HttpMethod.Post, "https://login.live.com/oauth20_token.srf") { Content = new FormUrlEncodedContent(parameters) };

                using (var response = await client.SendAsync(request))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var dataString = await response.Content.ReadAsStringAsync();
                        data = JObject.Parse(dataString);
                    }
                }
            }

            return data;
        }        

        public async Task<JObject> ExchangeRpsTicketForUserToken(string rpsTicket)
        {
            JObject data = null;

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("X-Xbl-Contract-Version", "2");

                var request = new HttpRequestMessage(HttpMethod.Post, "https://user.auth.xboxlive.com/user/authenticate");

                var parameters = new Dictionary<string, object> {
                    {"RelyingParty", "http://auth.xboxlive.com"},
                    {"TokenType", "JWT"},
                    {"Properties", new Dictionary<string, string> { {"AuthMethod", "RPS"}, {"SiteName", "user.auth.xboxlive.com"}, {"RpsTicket", "d=" + rpsTicket} } }
                };
                request.Content = new StringContent(JsonConvert.SerializeObject(parameters), Encoding.UTF8, "application/json");

                using (var response = await client.SendAsync(request))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var dataString = await response.Content.ReadAsStringAsync();
                        data = JObject.Parse(dataString);
                    }
                }
            }

            return data;
        }        

        public async Task<JObject> ExchangeTokenForXSTSToken(string userToken)
        {
            JObject data = null;

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("X-Xbl-Contract-Version", "2");

                var request = new HttpRequestMessage(HttpMethod.Post, "https://xsts.auth.xboxlive.com/xsts/authorize");

                var parameters = new Dictionary<string, object> {
                    {"RelyingParty", "http://xboxlive.com"},
                    //{"RelyingParty", "http://licensing.xboxlive.com"},
                    {"TokenType", "JWT"},
                    {"Properties", new Dictionary<string, object> { {"UserTokens", new string[]{userToken}}, {"SandboxId", "RETAIL"} } }
                };
                request.Content = new StringContent(JsonConvert.SerializeObject(parameters), Encoding.UTF8, "application/json");

                using (var response = await client.SendAsync(request))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var dataString = await response.Content.ReadAsStringAsync();
                        data = JObject.Parse(dataString);
                    }
                }
            }

            return data;
        }

        public async Task<List<string>> GetUserGameNames(string userHash, string xstsToken, ulong userXuid, DateTime? importLastRunDate)
        {
            var results = new List<string>();
            var items = await GetUserTitleHistory(userHash, xstsToken, userXuid);
            results = items.Where(obj => ((string)obj["titleType"]) == "Game" && (!importLastRunDate.HasValue || ((DateTime)obj["lastUnlock"]) >= importLastRunDate))
                            .OrderBy(obj => (DateTime)obj["lastUnlock"])
                            .Select(obj => (string)obj["name"]).ToList();
                            
            return results;
        }

        public async Task<JArray> GetUserTitleHistory(string userHash, string xstsToken, ulong userXuid, JArray results = null, string continuationToken = null)
        {
            if (results == null)
            {
                results = new JArray();
            }

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("XBL3.0", string.Format("x={0};{1}", userHash, xstsToken));
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("X-Xbl-Contract-Version", "2");

                var requestUrl = string.Format("https://achievements.xboxlive.com/users/xuid({0})/history/titles", userXuid);
                if (!string.IsNullOrWhiteSpace(continuationToken))
                {
                    var parameters = new Dictionary<string, string> {
                        {"continuationToken", continuationToken }
                    };
                    requestUrl = QueryHelpers.AddQueryString(requestUrl, parameters);
                }
                var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);

                using (var response = await client.SendAsync(request))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var dataString = await response.Content.ReadAsStringAsync();
                        var data = JObject.Parse(dataString);
                        var items = (JArray)data.GetValue("titles");
                        results.Merge(items);

                        continuationToken = (string)data["pagingInfo"]["continuationToken"];
                        if (!string.IsNullOrWhiteSpace(continuationToken))
                        {
                            await GetUserTitleHistory(userHash, xstsToken, userXuid, results, continuationToken);
                        }
                    }
                }
            }

            return results;
        }   

        public async Task<JObject> GetUserInventory(string userHash, string xstsToken, ulong userXuid)
        {
            JObject data = null;

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("XBL3.0", string.Format("x={0};{1}", userHash, xstsToken));
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue { NoCache = true, NoStore = true, MustRevalidate = true };
                client.DefaultRequestHeaders.Add("Pragma", "no-cache");
                client.DefaultRequestHeaders.Add("X-Xbl-Client-Type", "Companion");
                client.DefaultRequestHeaders.Add("X-Xbl-Client-Version", "2.0");                
                client.DefaultRequestHeaders.Add("X-Xbl-Contract-Version", "3");
                client.DefaultRequestHeaders.Add("X-Xbl-Device-Type", "Xbox360");
                client.DefaultRequestHeaders.Add("X-Xbl-IsAutomated-Client", "true");

                var request = new HttpRequestMessage(HttpMethod.Get, "https://inventory.xboxlive.com/users/me/inventory");

                // var parameters = new Dictionary<string, object> {
                //     {"RelyingParty", "http://licensing.xboxlive.com"}
                // };
                // request.Content = new StringContent(JsonConvert.SerializeObject(parameters), Encoding.UTF8, "application/json");

                using (var response = await client.SendAsync(request))
                {
                    var dataString = await response.Content.ReadAsStringAsync();
                    data = JObject.Parse(dataString);
                }
            }

            return data;
        }
        #endregion
    }
}
