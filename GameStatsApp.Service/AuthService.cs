using System;
using System.Collections.Generic;
using System.Net;
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
using GameStatsApp.Common.Extensions;
using System.Security.Cryptography;

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
                {"openid.realm", new Uri(redirectUri).GetLeftPart(UriPartial.Authority)},
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

        public async Task<bool> CheckSteamUserCommunityVisibility(string steamID)
        {
            bool result = false;

            using (HttpClient client = new HttpClient())
            {
                var requestUrl = string.Format("http://api.steampowered.com/ISteamUser/GetPlayerSummaries/v0002/");

                var clientID = _config.GetSection("Auth").GetSection("Steam").GetSection("ClientID").Value;
                var parameters = new Dictionary<string, string> {
                    {"key", clientID },
                    {"steamids", steamID },
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

                        if (data != null)
                        {
                            var items = (JArray)((JObject)data.GetValue("response")).GetValue("players");
                            if (items.Any()) {
                                result = (string)((JObject)items.FirstOrDefault()).GetValue("communityvisibilitystate") == "3";
                            }                                                       
                        }
                    }
                }
            }

            return result;
        }
        
        public async Task<JArray> GetSteamUserInventory(string steamID)
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
                        var items = (JArray)((JObject)data.GetValue("response")).GetValue("games");
                        results.Merge(items);
                    }
                }
            }

            return results;
        }           
        #endregion

        #region NSOAuth
        public Tuple<string, string> GetNSOAuthUrl()
        {
            var baseUrl = "https://accounts.nintendo.com/connect/1.0.0/authorize";
            
            byte[] bytes = new byte[36];
            new Random().NextBytes(bytes);
            var state = WebEncoders.Base64UrlEncode(bytes);

            byte[] bytes2 = new byte[32];
            new Random().NextBytes(bytes2);
            var codeVerifier = WebEncoders.Base64UrlEncode(bytes2);
            var codeChallenge = WebEncoders.Base64UrlEncode(SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(codeVerifier)));

            var parameters = new Dictionary<string,string>{
                {"state", state},
                {"redirect_uri", "npf71b963c1b7b6d119://auth"},
                {"client_id", "71b963c1b7b6d119"},                
                {"scope", "openid user user.birthday user.mii user.screenName"},
                {"response_type", "session_token_code"},
                {"session_token_code_challenge", codeChallenge},
                {"session_token_code_challenge_method", "S256"},
                {"theme", "login_form"}
            };

            var authUrl = QueryHelpers.AddQueryString(baseUrl, parameters);

            return new Tuple<string, string>(authUrl, codeVerifier);
        }

        public async Task<List<string>> GetNSOUserGameNames(string redirectUrl, string codeVerifier)
        {
            var results = new List<string>();
            var url = new Uri(redirectUrl.Replace("auth#", "auth?"));
            var items = QueryHelpers.ParseQuery(url.Query);
            var tokenCode = (string)items["session_token_code"];
            var sessionToken = await GetNSOSessionToken(tokenCode, codeVerifier);
            var response = await GetNSOApiToken(sessionToken);
            var userInfo = await GetNSOUserInfo((string)response.GetValue("access_token"));
            var response2 = await GetNSOFParam((string)response.GetValue("id_token"), 1, (string)userInfo.GetValue("id"));
            var response4 = await GetNSOApiLogin((string)response.GetValue("id_token"), (string)response2.GetValue("f"), (long)response2.GetValue("timestamp"), (string)response2.GetValue("request_id"), (string)userInfo.GetValue("country"), (string)userInfo.GetValue("birthday"), (string)userInfo.GetValue("language"));

            return results;
        }     

        public async Task<string> GetNSOSessionToken(string tokenCode, string codeVerifier)
        {
            var result = string.Empty;

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.UserAgent.ParseAdd("OnlineLounge/2.7.1 NASDKAPI Android");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue("en-US"));
                client.DefaultRequestHeaders.Add("Host", "accounts.nintendo.com");
                client.DefaultRequestHeaders.Add("Connection", "Keep-Alive");
                client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));

                var parameters = new Dictionary<string, string> {
                    {"client_id", "71b963c1b7b6d119"},
                    {"session_token_code", tokenCode},
                    {"session_token_code_verifier", codeVerifier}
                };
                var request = new HttpRequestMessage(HttpMethod.Post, "https://accounts.nintendo.com/connect/1.0.0/api/session_token") { Content = new FormUrlEncodedContent(parameters) };

                using (var response = await client.SendAsync(request))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var dataString = await response.Content.ReadAsStringAsync();
                        var data = JObject.Parse(dataString);

                        if (data != null)
                        {
                            result = (string)data.GetValue("session_token");
                        }                        
                    }
                }
            }

            return result;
        }

        public async Task<JObject> GetNSOApiToken(string sessionToken)
        {
            JObject data = null;

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.UserAgent.ParseAdd("OnlineLounge/2.7.1 NASDKAPI Android");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue("en-US"));
                client.DefaultRequestHeaders.Add("Host", "accounts.nintendo.com");
                client.DefaultRequestHeaders.Add("Connection", "Keep-Alive");
                client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));

                var request = new HttpRequestMessage(HttpMethod.Post, "https://accounts.nintendo.com/connect/1.0.0/api/token");

                var parameters = new Dictionary<string, object> {
                    {"client_id", "71b963c1b7b6d119"},
                    {"session_token", sessionToken},
                    {"grant_type", "urn:ietf:params:oauth:grant-type:jwt-bearer-session-token"}
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

        public async Task<JObject> GetNSOUserInfo(string apiToken)
        {
            JObject data = null;

            using (HttpClient client = new HttpClient(new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate }))
            {
                client.DefaultRequestHeaders.UserAgent.ParseAdd("OnlineLounge/2.7.1 NASDKAPI Android");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiToken);
                client.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue("en-US"));
                client.DefaultRequestHeaders.Add("Host", "api.accounts.nintendo.com");
                client.DefaultRequestHeaders.Add("Connection", "Keep-Alive");
                client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));

                var requestUrl = "https://api.accounts.nintendo.com/2.0.0/users/me";
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

        public async Task<JObject> GetNSOFParam(string idToken, int step, string userID)
        {
            JObject data = null;

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.UserAgent.ParseAdd("mybacklog.io/1.0");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var request = new HttpRequestMessage(HttpMethod.Post, "https://api.imink.app/f");

                var parameters = new Dictionary<string, object> {
                    {"token", idToken},
                    {"hash_method", step},
                    {"na_id", userID}
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

        public async Task<JObject> GetNSOApiLogin(string idToken, string fParam, long timestamp, string requestID, string userCountry, string userBirthday, string userLanguage)
        {
            JObject data = null;

            using (HttpClient client = new HttpClient(new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.GZip }))
            {
                client.DefaultRequestHeaders.UserAgent.ParseAdd("com.nintendo.znca/2.7.1 (Android/7.1.2)");
                client.DefaultRequestHeaders.Add("Host", "api-lp1.znc.srv.nintendo.net");
                client.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue("en-US"));
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer");
                client.DefaultRequestHeaders.Add("X-ProductVersion", "2.7.1");
                client.DefaultRequestHeaders.Add("Connection", "Keep-Alive");
                client.DefaultRequestHeaders.Add("X-Platform", "Android");

                var request = new HttpRequestMessage(HttpMethod.Post, "https://api-lp1.znc.srv.nintendo.net/v3/Account/Login");

                var parameters = new Dictionary<string, object> {
                    {"f", fParam},
                    {"naIdToken", idToken},
                    {"timestamp", timestamp},
                    {"requestId", requestID},
                    {"naCountry", userCountry},
                    {"naBirthday", userBirthday},
                    {"language", userLanguage}
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

        public async Task<JArray> GetMicrosoftUserTitleHistory(string userHash, string xstsToken, ulong userXuid, JArray results = null, string continuationToken = null)
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
                client.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue("en-US"));

                var requestUrl = string.Format("https://titlehub.xboxlive.com/users/xuid({0})/titles/titlehistory/decoration/scid,image,detail", userXuid);
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

                        continuationToken = (string)data["pagingInfo"]?["continuationToken"];
                        if (!string.IsNullOrWhiteSpace(continuationToken))
                        {
                            await GetMicrosoftUserTitleHistory(userHash, xstsToken, userXuid, results, continuationToken);
                        }
                    }
                }
            }

            return results;
        }  

        private async Task<JArray> GetMicrosoftUserAchievementHistory(string userHash, string xstsToken, ulong userXuid, JArray results = null, string continuationToken = null)
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
                            await GetMicrosoftUserAchievementHistory(userHash, xstsToken, userXuid, results, continuationToken);
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
                var parameters = new Dictionary<string, string> {
                    {"expandSatisfyingEntitlements", "true"},             
                    {"availability", "All"}
                };                        
                if (!string.IsNullOrWhiteSpace(continuationToken))
                {
                    parameters.Add("continuationToken", continuationToken);
                }        
                requestUrl = QueryHelpers.AddQueryString(requestUrl, parameters);

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
                            await GetMicrosoftUserInventory(userHash, xstsToken, userXuid, results, continuationToken);
                        }
                    }
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

        public async Task<bool> ValidateGoogleRecaptcha(string token)
        {
            bool result = false;
            var clientSecret = _config.GetSection("Auth").GetSection("Google").GetSection("RecaptchaSecret").Value;

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var parameters = new Dictionary<string,string>{
                    {"secret", clientSecret},
                    {"response", token}
                };

                var request = new HttpRequestMessage(HttpMethod.Post, "https://www.google.com/recaptcha/api/siteverify") { Content = new FormUrlEncodedContent(parameters) };

                using (var response = await client.SendAsync(request))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var dataString = await response.Content.ReadAsStringAsync();
                        var data = JObject.Parse(dataString);
                        
                        if (data != null)
                        {
                            result = (bool)data.GetValue("success");
                        }
                    }
                }
            }

            return result;
        }                
        #endregion
   }
}
