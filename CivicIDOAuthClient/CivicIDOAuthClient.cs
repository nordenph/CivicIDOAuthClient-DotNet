using DotNetOpenAuth.AspNet.Clients;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace Accela.OAuth.Client
{
    public class CivicIDOAuthClient : OAuth2Client
    {
        #region End Points

        private const string AuthorizationEndPoint = "https://auth.accela.com/oauth2/authorize";
        private const string TokenEndPoint = "https://apis.accela.com/oauth2/token";
        private const string UserProfileEndPoint = "https://apis.accela.com/v3/users/me";

        #endregion

        private string _profileScope = "get_user_profile";

        public string AppId { get; protected set; }
        public string AppSecret { get; protected set; }
        public string Environment { get; protected set; }
        public string[] Scopes { get; protected set; }
        public string AgencyName { get; protected set; }

        // Initializes provider with the default name: CivicIDProvider
        public CivicIDOAuthClient(string appId, string appSecret, string environment, string agencyName, string[] scopes)
            : this("CivicIDProvider", appId, appSecret, environment, agencyName, scopes)
        {
        }

        // Initializes provider with custom name
        public CivicIDOAuthClient(string providerName, string appId, string appSecret, string environment, string agencyName, string[] scopes)
            : base(providerName)
        {
            if (!string.IsNullOrEmpty(appId))
                this.AppId = appId;
            else
                throw new Exception("AppId is required");
            
            if (!string.IsNullOrEmpty(appSecret))
                this.AppSecret = appSecret;
            else
                throw new Exception("AppSecret is required");

            if (!string.IsNullOrEmpty(environment))
                this.Environment = environment;
            else
                throw new Exception("Environment is required");

            if (!string.IsNullOrEmpty(agencyName))
                this.AgencyName = agencyName;

            var scopeList = new List<string>();

            if (scopes != null
                && scopes.Any())
            {
                // check to see if this.Scope already has get_user_profile. If not, add profile scope
                if (!Array.Exists(scopes, delegate(string str) { return str.Equals(_profileScope, StringComparison.OrdinalIgnoreCase); }))
                {
                    // add profile scope
                    scopeList.Add(_profileScope);
                }

                // copy over scopes passed in by the client
                scopeList.AddRange(scopes);
            }
            else
                scopeList.Add(_profileScope);

            this.Scopes = scopeList.ToArray();
        }
        
        /// <summary>
        /// Generates service URL the user should be directed to
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        protected override Uri GetServiceLoginUrl(Uri returnUrl)
        {
            var uriBuilder = new UriBuilder(AuthorizationEndPoint);
            var queryString = new StringBuilder();
            var scopesString = string.Join(" ", this.Scopes);

            queryString.Append("response_type=code");
            queryString.Append("&client_id=" + this.AppId);
            queryString.Append("&redirect_uri=" + HttpUtility.UrlEncode(returnUrl.AbsoluteUri));
            queryString.Append("&environment=" + this.Environment.ToUpper());
            queryString.Append("&scope=" + scopesString);

            if (!string.IsNullOrEmpty(this.AgencyName))
                queryString.Append("&agency_name=" + this.AgencyName);

            uriBuilder.Query = queryString.ToString();
            
            return uriBuilder.Uri;
        }

        /// <summary>
        /// Sends request to get user's CivicID profile
        /// </summary>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        protected override IDictionary<string, string> GetUserData(string accessToken)
        {
            var civicUser = new User();

            using (var client = new WebClient())
            {
                //set headers
                client.Headers[HttpRequestHeader.ContentType] = "application/json; charset=utf-8;";
                client.Headers[HttpRequestHeader.Accept] = "application/json";
                client.Headers[HttpRequestHeader.Authorization] = accessToken;
                client.Headers.Add("x-accela-appid", this.AppId);

                var responseBody = client.DownloadString(UserProfileEndPoint);

                civicUser = JsonConvert.DeserializeObject<User>(responseBody);
            }

            var userData = new Dictionary<string, string>();

            //Fill the Dictionary with user's data. This will be available through ExytraData Dictionary of the DotNetOpenAuth.AspNet.AuthenticationResult instance
            if (civicUser != null)
            {
                userData.Add("id", civicUser.LoginName);
                userData.Add("civicId", civicUser.Id);
                userData.Add("email", civicUser.Email);
                userData.Add("login", civicUser.LoginName);
                userData.Add("firstName", civicUser.FirstName);
                userData.Add("lastName", civicUser.LastName);

                userData.Add("agencyName", this.AgencyName.ToLower());

                userData.Add("countryCode", civicUser.CountryCode);
                userData.Add("city", civicUser.City);
                userData.Add("streetAddress", civicUser.StreetAddress);
                userData.Add("state", civicUser.State);
                userData.Add("postalCode", civicUser.PostalCode);

                userData.Add("phoneCountryCode", civicUser.PhoneCountryCode);
                userData.Add("phoneAreaCode", civicUser.PhoneAreaCode);
                userData.Add("phoneNumber", civicUser.PhoneNumber);

                userData.Add("avatarUrl", civicUser.AvatarUrl);
            }

            return userData;
        }

        /// <summary>
        /// Exchanges authorization code for the access token
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <param name="authorizationCode"></param>
        /// <returns></returns>
        protected override string QueryAccessToken(Uri returnUrl, string authorizationCode)
        {
            var queryString = new StringBuilder();

            queryString.Append("client_id=" + this.AppId);
            queryString.Append("&client_secret=" + this.AppSecret);
            queryString.Append("&grant_type=authorization_code");
            queryString.Append("&code=" + authorizationCode);
            queryString.Append("&redirect_uri=" + HttpUtility.UrlEncode(returnUrl.AbsoluteUri));

            var query = queryString.ToString();

            var tokenRequest = WebRequest.Create(TokenEndPoint);
            tokenRequest.ContentType = "application/x-www-form-urlencoded";
            tokenRequest.ContentLength = query.Length;
            tokenRequest.Method = "POST";
            //set headers
            tokenRequest.Headers.Add("x-accela-appid", this.AppId);

            var response = RequestHelper.SendPOSTRequest<AccessToken>(tokenRequest, query);

            if (response != null)
                return response.Token;

            return null;
        }
    }
}