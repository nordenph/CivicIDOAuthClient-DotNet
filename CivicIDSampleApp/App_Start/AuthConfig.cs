using Accela.OAuth.Client;
using Microsoft.Web.WebPages.OAuth;

namespace CivicIDSampleApp
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

            //var appInfo = new AppInfo
            //{
            //    AgencyName = "agency name (for Agency app) or empty string for citizen app",
            //    ApplicationId = "your app id (Citizen or Agency app)",
            //    ApplicationSecret = "your app secret (Citizen or Agency app)",
            //    Environment = "your environment", //e.g. TEST, PROD
            //    Scopes = null
            //};

            //var endPoints = new EndPointsConfig
            //{
            //    AuthorizationEndPoint = "auth end-point", //e.g. "https://auth.accela.com/oauth2/authorize"
            //    TokenEndPoint = "exchange token end-point", //e.g. "https://apis.accela.com/oauth2/token"
            //    UserProfileEndPoint = "user profile end-point", //e.g. "https://apis.accela.com/v3/users/me"
            //};

            //OAuthWebSecurity.RegisterClient(
            //    new CivicIDOAuthClient(appInfo, endPoints), "Civic ID", null);

            //OAuthWebSecurity.RegisterGoogleClient();
        }
    }
}
