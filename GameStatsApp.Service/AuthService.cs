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
using Google.Apis.Auth;

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

        #region SteamAuth
        public string GetSteamAuthUrl(string redirectUri)
        {
            var baseUrl = string.Format("https://steamcommunity.com/openid/login");

            var parameters = new Dictionary<string,string>{
                {"openid.ns", "http://specs.openid.net/auth/2.0"},
                {"openid.claimed_id", "http://specs.openid.net/auth/2.0/identifier_select"},
                {"openid.identity", "http://specs.openid.net/auth/2.0/identifier_select"},
                {"openid.return_to", redirectUri},
                {"openid.realm", "https://localhost:5000"},
                {"openid.mode", "checkid_setup"},
            };

            var authUrl = QueryHelpers.AddQueryString(baseUrl, parameters);
            
            return authUrl;
        }

        public async Task<TokenResponse> AuthenticateSteam(Dictionary<string, string> results)
        {
            TokenResponse result = null;
            var isValid = await ValidateSteam(results);

            if (isValid)
            {
                var steamID = new Uri(results["openid.claimed_id"]).Segments.LastOrDefault();      
                if(!string.IsNullOrWhiteSpace(steamID))
                {
                    result = new TokenResponse() { AccountUserID = steamID };
                }
            }

            return result;
        }    

        private async Task<bool> ValidateSteam(Dictionary<string, string> parameters)
        {
            bool result = false;
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                parameters["openid.mode"] = "check_authentication";
                var request = new HttpRequestMessage(HttpMethod.Post, "https://steamcommunity.com/openid/login") { Content = new FormUrlEncodedContent(parameters) };

                using (var response = await client.SendAsync(request))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var dataString = await response.Content.ReadAsStringAsync();
                        result = dataString.Contains("true");
                    }
                }
            }

            return result;
        }    

        public async Task<List<string>> GetSteamUserGameNames(string steamID)
        {
            var results = new List<string>();
            var items = await GetSteamUserInventory(steamID);
            results = items.OrderBy(obj => DateTimeOffset.FromUnixTimeSeconds((long)obj["rtime_last_played"]).UtcDateTime)
                           .Select(obj => (string)obj["name"]).ToList();

            results = results.GroupBy(g => new { g })
                .Select(i => i.First())
                .ToList();
                                      
            return results;
        }       

        private async Task<JArray> GetSteamUserInventory(string steamID)
        {
            var results = new JArray();

            using (HttpClient client = new HttpClient())
            {
                var requestUrl = string.Format("http://api.steampowered.com/IPlayerService/GetOwnedGames/v0001/");

                var clientID = _config.GetSection("Auth").GetSection("Steam").GetSection("ClientID").Value;           
                var parameters = new Dictionary<string, string> {
                    {"key", clientID },
                    {"steamid", steamID },
                    {"include_appinfo", "true"},
                    {"include_played_free_games", "true"},
                    {"format", "json" },                       
                };
                requestUrl = QueryHelpers.AddQueryString(requestUrl, parameters);
                var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);

                using (var response = await client.SendAsync(request))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var dataString = await response.Content.ReadAsStringAsync();
                        var data = JObject.Parse(dataString);
                        results = (JArray)data["response"]["games"];
                    }
                }
            }

            return results;
        }           
        #endregion

        #region MicrosoftAuth
        public string GetMicrosoftAuthUrl(string redirectUri)
        {
            var baseUrl = "https://login.live.com/oauth20_authorize.srf";
            var clientID = _config.GetSection("Auth").GetSection("Microsoft").GetSection("ClientID").Value;
            
            var parameters = new Dictionary<string,string>{
                {"client_id", clientID},
                {"scope", "XboxLive.signin XboxLive.offline_access"},
                {"response_type", "code"},
                {"redirect_uri", redirectUri},
            };

            var authUrl = QueryHelpers.AddQueryString(baseUrl, parameters);
            
            return authUrl;
        }

        public async Task<TokenResponse> AuthenticateMicrosoft(string code, string redirectUri)
        {
            var accessResponse = await ExchangeCodeForAccessToken(code, redirectUri);
            var result = await GetTokenResponse(accessResponse);

            return result;
        }

        public async Task<TokenResponse> ReAuthenticateMicrosoft(string refreshToken)
        {
            var accessResponse = await RefreshAccessToken(refreshToken);
            var result = await GetTokenResponse(accessResponse);

            return result;
        }

        private async Task<TokenResponse> GetTokenResponse(JObject accessResponse)
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

        private async Task<JObject> ExchangeCodeForAccessToken(string code, string redirectUri)
        {
            JObject data = null;
            var clientID = _config.GetSection("Auth").GetSection("Microsoft").GetSection("ClientID").Value;
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

        private async Task<JObject> RefreshAccessToken(string refreshToken)
        {
            JObject data = null;
            var clientID = _config.GetSection("Auth").GetSection("Microsoft").GetSection("ClientID").Value;
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

        private async Task<JObject> ExchangeRpsTicketForUserToken(string rpsTicket)
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

        private async Task<JObject> ExchangeTokenForXSTSToken(string userToken)
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

        public async Task<List<string>> GetMicrosoftUserGameNames(string userHash, string xstsToken, ulong userXuid, DateTime? importLastRunDate)
        {
            var results = new List<string>();
            var items = await GetMicrosoftUserTitleHistory(userHash, xstsToken, userXuid);
            results = items.Where(obj => ((string)obj["titleType"]) == "Game" && (!importLastRunDate.HasValue || ((DateTime)obj["lastUnlock"]) >= importLastRunDate))
                            .OrderBy(obj => (DateTime)obj["lastUnlock"])
                            .Select(obj => (string)obj["name"]).ToList();

            results = results.GroupBy(g => new { g })
                .Select(i => i.First())
                .ToList();
                                      
            return results;
        }
        
        private async Task<JArray> GetMicrosoftUserTitleHistory(string userHash, string xstsToken, ulong userXuid, JArray results = null, string continuationToken = null)
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
                            await GetMicrosoftUserTitleHistory(userHash, xstsToken, userXuid, results, continuationToken);
                        }
                    }
                }
            }

            return results;
        }   

        private async Task<JArray> GetMicrosoftUserInventory(string userHash, string xstsToken, ulong userXuid, JArray results = null, string continuationToken = null)
        {
            if (results == null)
            {
                results = new JArray();
            }

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("XBL3.0", string.Format("x={0};{1}", userHash, xstsToken));
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue { NoCache = true, NoStore = true, MustRevalidate = true };
                client.DefaultRequestHeaders.Add("Pragma", "no-cache");
                client.DefaultRequestHeaders.Add("X-Xbl-Client-Type", "Companion");
                client.DefaultRequestHeaders.Add("X-Xbl-Client-Version", "2.0");                
                client.DefaultRequestHeaders.Add("X-Xbl-Contract-Version", "3");
                client.DefaultRequestHeaders.Add("X-Xbl-Device-Type", "iPhone");
                client.DefaultRequestHeaders.Add("X-Xbl-IsAutomated-Client", "true");

                var requestUrl = "https://inventory.xboxlive.com/users/me/inventory";
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
                    // if (response.IsSuccessStatusCode)
                    // {
                        var blah = response.IsSuccessStatusCode;
                        var dataString = await response.Content.ReadAsStringAsync();
                        var data = JObject.Parse(dataString);
                        var items = (JArray)data.GetValue("titles");
                        results.Merge(items);

                        continuationToken = (string)data["pagingInfo"]["continuationToken"];
                        if (!string.IsNullOrWhiteSpace(continuationToken))
                        {
                            await GetMicrosoftUserInventory(userHash, xstsToken, userXuid, results, continuationToken);
                        }
                    // }
                }
            }

            return results;
        }
        #endregion

        #region SocialAuth
        public async Task<SocialTokenResponse> ValidateSocialToken(string accessToken, int socialAccountTypeID)
        {
            SocialTokenResponse result = null;
            if (socialAccountTypeID == (int)SocialAccountType.Google)
            {
                var response = await GoogleJsonWebSignature.ValidateAsync(accessToken);
                if (response != null)
                {
                    result = new SocialTokenResponse() { Email = response.Email };
                }
            }
            else
            {
                var response = await GetFacebookResponse(accessToken);
                if (response != null)
                {
                    result = new SocialTokenResponse() { Email = (string)response.GetValue("email") };
                }
            }

            return result;
        }
        
        private async Task<JObject> GetFacebookResponse(string accessToken)
        {
            JObject data = null;

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var requestUrl = "https://graph.facebook.com/me";
                var parameters = new Dictionary<string, string> {
                    {"access_token", accessToken },
                    {"fields", "name,email"}            
                };                
                requestUrl = QueryHelpers.AddQueryString(requestUrl, parameters);
                var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);

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
        #endregion
   }
}
