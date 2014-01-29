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
        private string _profileScope = "";

        public AppInfo AppInfo { get; protected set; }
        public EndPointsConfig EndPoints { get; protected set; }

        // Initializes provider with the default name: CivicIDProvider
        public CivicIDOAuthClient(AppInfo appInfo, EndPointsConfig endPoints)
            : this("CivicIDProvider", appInfo, endPoints)
        {   
        }

        // Initializes provider with custom name
        public CivicIDOAuthClient(string providerName, AppInfo appInfo, EndPointsConfig endPoints)
            : base(providerName)
        {
            if (appInfo == null)
                throw new Exception("Application Info is required");

            if (endPoints == null)
                throw new Exception("End-points are required");

            this.EndPoints = endPoints;
            this.AppInfo = appInfo;
            
            if (string.IsNullOrEmpty(appInfo.ApplicationId))
                throw new Exception("Application Id is required");
            
            if (string.IsNullOrEmpty(appInfo.ApplicationSecret))
                throw new Exception("Application Secret is required");

            if (string.IsNullOrEmpty(appInfo.Environment))
                throw new Exception("Environment is required");

            var scopeList = new List<string>();

            if (endPoints.UserProfileEndPoint.Contains("v3"))
                _profileScope = "get_user_profile";
            else if (endPoints.UserProfileEndPoint.Contains("v4"))
                _profileScope = "get_my_profile";

            if (this.AppInfo.Scopes != null
                && this.AppInfo.Scopes.Any())
            {
                // check to see if this.Scope already has get_user_profile. If not, add profile scope
                if (!Array.Exists(this.AppInfo.Scopes, delegate(string str) { return str.Equals(_profileScope, StringComparison.OrdinalIgnoreCase); }))
                {
                    // add profile scope
                    scopeList.Add(_profileScope);
                }

                // copy over scopes passed in by the client
                scopeList.AddRange(this.AppInfo.Scopes);
            }
            else
                scopeList.Add(_profileScope);

            this.AppInfo.Scopes = scopeList.ToArray();
        }
        
        /// <summary>
        /// Generates service URL the user should be directed to
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        protected override Uri GetServiceLoginUrl(Uri returnUrl)
        {
            var uriBuilder = new UriBuilder(this.EndPoints.AuthorizationEndPoint);
            var queryString = new StringBuilder();
            var scopesString = string.Join(" ", this.AppInfo.Scopes);

            queryString.Append("response_type=code");
            queryString.Append("&client_id=" + this.AppInfo.ApplicationId);
            queryString.Append("&redirect_uri=" + HttpUtility.UrlEncode(returnUrl.AbsoluteUri));
            queryString.Append("&environment=" + this.AppInfo.Environment.ToUpper());
            queryString.Append("&scope=" + scopesString);

            if (!string.IsNullOrEmpty(this.AppInfo.AgencyName))
                queryString.Append("&agency_name=" + this.AppInfo.AgencyName);

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
                client.Headers.Add("x-accela-appid", this.AppInfo.ApplicationId);

                var responseBody = client.DownloadString(this.EndPoints.UserProfileEndPoint);

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

                userData.Add("agencyName", this.AppInfo.AgencyName.ToLower());

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

            queryString.Append("client_id=" + this.AppInfo.ApplicationId);
            queryString.Append("&client_secret=" + this.AppInfo.ApplicationSecret);
            queryString.Append("&grant_type=authorization_code");
            queryString.Append("&code=" + authorizationCode);
            queryString.Append("&redirect_uri=" + HttpUtility.UrlEncode(returnUrl.AbsoluteUri));

            var query = queryString.ToString();

            var tokenRequest = WebRequest.Create(this.EndPoints.TokenEndPoint);
            tokenRequest.ContentType = "application/x-www-form-urlencoded";
            tokenRequest.ContentLength = query.Length;
            tokenRequest.Method = "POST";
            //set headers
            tokenRequest.Headers.Add("x-accela-appid", this.AppInfo.ApplicationId);

            var response = RequestHelper.SendPOSTRequest<AccessToken>(tokenRequest, query);

            if (response != null)
                return response.Token;

            return null;
        }
    }
}