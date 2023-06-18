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

        public string GetWindowsLiveAuthUrl()
        {
            var baseUrl = "https://login.live.com/oauth20_authorize.srf";
            var redirectUri = _config.GetSection("Auth").GetSection("Microsoft").GetSection("RedirectUri").Value;
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

        // public string GetRefreshWindowsLiveAuthUrl(AccessToken refreshToken)
        // {
        //     var baseUrl = "https://login.live.com/oauth20_authorize.srf";
        //     var clientID = _config.GetSection("Auth").GetSection("Microsoft").GetSection("ClientId").Value;
            
        //     var parameters = new Dictionary<string,string>{
        //         {"client_id", clientID},
        //         {"scope", "XboxLive.signin XboxLive.offline_access"},
        //         {"grant_type", "refresh_token"},
        //         {"refresh_token", refreshToken.Jwt}
        //     };

        //     var authUrl = QueryHelpers.AddQueryString(baseUrl, parameters);
            
        //     return authUrl;
        // }

        public async void Authenticate(string code)
        {
            var accessResponse = await ExchangeCodeForAccessToken(code);
            RpsTicket = (string)accessResponse.GetValue("access_token");
            RefreshToken = (string)accessResponse.GetValue("refresh_token");

            var userResponse = await ExchangeRpsTicketForUserToken(RpsTicket);
            UserToken = (string)userResponse.GetValue("Token");

            var xstsResponse = await ExchangeTokenForXSTSToken(UserToken);
            XSTSTResponse = new XSTSTokenResponse() { Token = (string)xstsResponse.GetValue("Token"),
                                                      IssueInstant = (DateTime)xstsResponse.GetValue("IssueInstant"),
                                                      NotAfter = (DateTime)xstsResponse.GetValue("NotAfter"),
                                                      UserInformation = ((JObject)xstsResponse["DisplayClaims"]["xui"][0]).ToObject<XboxUserInformation>() };

            var userInventoryResponse = await GetUserInventory(XSTSTResponse.UserInformation.Userhash, XSTSTResponse.Token, XSTSTResponse.UserInformation.XboxUserId);
        }

        public async Task<JObject> ExchangeCodeForAccessToken(string code)
        {
            JObject data = null;
            var clientID = _config.GetSection("Auth").GetSection("Microsoft").GetSection("ClientId").Value;
            var redirectUri = _config.GetSection("Auth").GetSection("Microsoft").GetSection("RedirectUri").Value;
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
                    //{"RelyingParty", "http://xboxlive.com"},
                    {"RelyingParty", "http://licensing.xboxlive.com"},
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
                client.DefaultRequestHeaders.Add("X-Xbl-Contract-Version", "2");
                client.DefaultRequestHeaders.Add("X-Xbl-Device-Type", "WindowsPhone");
                client.DefaultRequestHeaders.Add("X-Xbl-IsAutomated-Client", "true");

                var request = new HttpRequestMessage(HttpMethod.Get, "https://inventory.xboxlive.com/users/me/inventory");

                using (var response = await client.SendAsync(request))
                {
                    var dataString = await response.Content.ReadAsStringAsync();
                    data = JObject.Parse(dataString);
                }
            }

            return data;
        }          

        /*
        public async Task<JObject> GetUserInventory(string userHash, string xstsToken, ulong userXuid)
        {
            JObject data = null;

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("XBL3.0", string.Format("x={0};{1}", userHash, xstsToken));
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("X-Xbl-Contract-Version", "2");

                var request = new HttpRequestMessage(HttpMethod.Get, string.Format("https://achievements.xboxlive.com/users/xuid({0})/history/titles", userXuid));

                using (var response = await client.SendAsync(request))
                {
                    var dataString = await response.Content.ReadAsStringAsync();
                    data = JObject.Parse(dataString);
                }
            }

            return data;
        }   
        */
        /*
        public async void ExchangeCodeForAccessToken(string code)
        {
            var clientID = _config.GetSection("Auth").GetSection("Microsoft").GetSection("ClientId").Value;
            var redirectUri = _config.GetSection("Auth").GetSection("Microsoft").GetSection("RedirectUri").Value;
            var clientSecret = _config.GetSection("Auth").GetSection("Microsoft").GetSection("ClientSecret").Value;

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.2; WOW64; rv:33.0) Gecko/20100101 Firefox/33.0");
                client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
                client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));
                client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("compress"));
                client.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue { NoCache = true, NoStore = true, MustRevalidate = true };
                client.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue("en-US"));
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));

                //var request = new HttpRequestMessage(HttpMethod.Post, "https://login.live.com/oauth20_token.srf");
                // var parameters = new Dictionary<string,string>{
                //     {"code", code},
                //     {"client_id", null},
                //     {"grant_type", "authorization_code"},
                //     {"redirect_uri", null},
                //     {"scope", null}
                // };

                var nvc = new List<KeyValuePair<string, string>>();
                nvc.Add(new KeyValuePair<string, string>("code", code));
                nvc.Add(new KeyValuePair<string, string>("client_id", clientID));
                nvc.Add(new KeyValuePair<string, string>("grant_type", "authorization_code"));
                nvc.Add(new KeyValuePair<string, string>("redirect_uri", redirectUri));
                nvc.Add(new KeyValuePair<string, string>("scope", "XboxLive.signin XboxLive.offline_access"));
                nvc.Add(new KeyValuePair<string, string>("client_secret", clientSecret));

                var request = new HttpRequestMessage(HttpMethod.Post, "https://login.live.com/oauth20_token.srf") { Content = new FormUrlEncodedContent(nvc) };
                //var jsonRequest = JsonConvert.SerializeObject(parameters);
                //request.Content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");         
                //request.Headers.Add("User-Agent", "XboxReplay; XboxLiveAuth/4.0");
                //request.Headers.Add("Accept", "application/json");
                //request.Headers.Add("Content-Type", "application/x-www-form-urlencoded");   
                // request.Headers.Add("Cache-Control", "no-cache, no-store, must-revalidate");
                // request.Headers.Add("Accept-Encoding", "gzip, deflate, compress");
                // request.Headers.Add("Accept-Language", "en-US");

                using (var response = await client.SendAsync(request))
                {
                    var dataString = await response.Content.ReadAsStringAsync();
                    var data = JsonConvert.DeserializeObject(dataString);
                    //result = new AccessToken() { Jwt = data.Token, Issued = data.IssueInstant, Expires = data.NotAfter };
                }
            }
        }
        

        // public void SetAccessToken(string url)
        // {
        //     var windowsLiveTokens = ParseWindowsLiveResponse(url);
        //     AccessToken = new AccessToken() { Issued = windowsLiveTokens.CreationTime, Expires = windowsLiveTokens.CreationTime + TimeSpan.FromSeconds(windowsLiveTokens.ExpiresIn), Jwt = windowsLiveTokens.AccessToken };
        //     RefreshToken = new AccessToken() { Issued = windowsLiveTokens.CreationTime, Expires = windowsLiveTokens.CreationTime + TimeSpan.FromDays(14), Jwt = windowsLiveTokens.AccessToken };
        // }

        public WindowsLiveResponse ParseWindowsLiveResponse(string url)
        {
            WindowsLiveResponse result = null;
            var queryParams = new NameValueCollection();
            var redirectUri = _config.GetSection("Auth").GetSection("Microsoft").GetSection("RedirectUri").Value;

            if (!string.IsNullOrWhiteSpace(url))
            {
                var urlFragment = new Uri(url).Fragment;
                if (url.StartsWith(redirectUri) && urlFragment.StartsWith("#access_token"))
                {
                    urlFragment = urlFragment.Substring(1);
                    queryParams  = System.Web.HttpUtility.ParseQueryString(urlFragment);
                }
            }

            result = new WindowsLiveResponse()
            { 
                ExpiresIn = !string.IsNullOrWhiteSpace(queryParams["expires_in"]) ? Convert.ToInt32(queryParams["expires_in"]) : 0,
                AccessToken = queryParams["access_token"],
                TokenType = queryParams["token_type"],
                Scope = queryParams["scope"],
                RefreshToken = queryParams["refresh_token"],
                UserId = queryParams["user_id"]     
            };

            return result;
        }

        // public async Task<bool> AuthenticateAsync()
        // {
        //     WindowsLiveResponse windowsLiveTokens = await RefreshLiveTokenAsync(RefreshToken);
        //     AccessToken = new AccessToken() { Issued = windowsLiveTokens.CreationTime, Expires = windowsLiveTokens.CreationTime + TimeSpan.FromSeconds(windowsLiveTokens.ExpiresIn), Jwt = windowsLiveTokens.AccessToken };
        //     RefreshToken = new AccessToken() { Issued = windowsLiveTokens.CreationTime, Expires = windowsLiveTokens.CreationTime + TimeSpan.FromDays(14), Jwt = windowsLiveTokens.AccessToken };
        //     UserToken = await AuthenticateXASUAsync(AccessToken);
        //     var xstsResult = await AuthenticateXSTSAsync(UserToken);
        //     XToken = xstsResult.Item1;
        //     UserInformation = xstsResult.Item2;

        //     return true;
        // }

        public async Task<WindowsLiveResponse> RefreshLiveTokenAsync(AccessToken refreshToken)
        {
            WindowsLiveResponse result = null;

            using (HttpClient client = new HttpClient())
            {
                var requestUrl = GetRefreshWindowsLiveAuthUrl(refreshToken);
                var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);

                using (var response = await client.SendAsync(request))
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        var dataString = await response.Content.ReadAsStringAsync();
                        result = JsonConvert.DeserializeObject<WindowsLiveResponse>(dataString);
                    }
                }
            }

            return result;
        }        

        public async Task<AccessToken> AuthenticateXASUAsync(AccessToken accessToken)
        {
           AccessToken result = null;

           using (HttpClient client = new HttpClient())
           {
                var request = new HttpRequestMessage(HttpMethod.Post, "https://user.auth.xboxlive.com/user/authenticate");
                var xasuRequest = new XASURequest() { RelyingParty = "http://auth.xboxlive.com", TokenType = "JWT", Properties = new XASUProperties() { AuthMethod = "RPS", SiteName = "user.auth.xboxlive.com", RpsTicket = accessToken.Jwt } };               
                var jsonRequest = JsonConvert.SerializeObject(xasuRequest);
                request.Headers.Add("x-xbl-contract-version", "1");
                request.Content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                using (var response = await client.SendAsync(request))
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        var dataString = await response.Content.ReadAsStringAsync();
                        var data = JsonConvert.DeserializeObject<XASResponse>(dataString);
                        result = new AccessToken() { Jwt = data.Token, Issued = data.IssueInstant, Expires = data.NotAfter };
                    }
                }
           }

           return result;
        }

        public async Task<Tuple<AccessToken, XboxUserInformation>> AuthenticateXSTSAsync(AccessToken userToken)
        {
           Tuple<AccessToken, XboxUserInformation> result = null;

           using (HttpClient client = new HttpClient())
           {
                var request = new HttpRequestMessage(HttpMethod.Post, "https://user.auth.xboxlive.com/xsts/authorize");
                var xstsRequest = new XSTSRequest() { RelyingParty = "http://xboxlive.com", TokenType = "JWT", Properties = new Dictionary<string, object> { {"UserTokens", new string[]{userToken.Jwt}}, {"SandboxId", "RETAIL"} } };               
                var jsonRequest = JsonConvert.SerializeObject(xstsRequest);
                request.Headers.Add("x-xbl-contract-version", "1");
                request.Content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                using (var response = await client.SendAsync(request))
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        var dataString = await response.Content.ReadAsStringAsync();
                        var data = JsonConvert.DeserializeObject<XASResponse>(dataString);
                        var xtoken = new AccessToken() { Jwt = data.Token, Issued = data.IssueInstant, Expires = data.NotAfter };
                        var userInfo = data.DisplayClaims["xui"][0];
                        result = new Tuple<AccessToken, XboxUserInformation>(xtoken, userInfo);
                    }
                }
           }

           return result;
        }
        */
    }
}
