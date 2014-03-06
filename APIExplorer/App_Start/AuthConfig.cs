using Accela.OAuth.Client;
using Microsoft.Web.WebPages.OAuth;
using System.Configuration;

namespace APIExplorer
{
    public static class AuthConfig
    {
        public static void RegisterAuth()
        {
            // To let users of this site log in using their accounts from  other sites such as Microsoft, Facebook, and Twitter,
            // you must update this site. For more information visit http://go.microsoft.com/fwlink/?LinkID=252166

            //OAuthWebSecurity.RegisterMicrosoftClient(
            //    clientId: "",
            //    clientSecret: "");

            //OAuthWebSecurity.RegisterTwitterClient(
            //    consumerKey: "",
            //    consumerSecret: "");

            //OAuthWebSecurity.RegisterFacebookClient(
            //    appId: "",
            //    appSecret: "");

            var appInfo = new AppInfo
            {
                AgencyName = ConfigurationManager.AppSettings["agencyId"],
                ApplicationId = ConfigurationManager.AppSettings["appId"],
                ApplicationSecret = ConfigurationManager.AppSettings["appSecret"],
                Environment = "TEST", //e.g. TEST, PROD
                Scopes = new string[] { "create_record", "get_records", "get_record" }
            };

            var endPoints = new EndPointsConfig
            {
                AuthorizationEndPoint = "https://auth.accela.com/oauth2/authorize", //e.g. "https://auth.accela.com/oauth2/authorize"
                TokenEndPoint = "https://apis.accela.com/oauth2/token", //e.g. "https://apis.accela.com/oauth2/token"
                UserProfileEndPoint = "https://apis.accela.com/v3/users/me", //e.g. "https://apis.accela.com/v3/users/me"
            };

            OAuthWebSecurity.RegisterClient(
                new CivicIDOAuthClient(appInfo, endPoints), "Civic ID", null);

            //OAuthWebSecurity.RegisterGoogleClient();
        }
    }
}
