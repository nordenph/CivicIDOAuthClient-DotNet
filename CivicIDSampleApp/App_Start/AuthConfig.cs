﻿using Accela.OAuth.Client;
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

            //OAuthWebSecurity.RegisterClient(
            //    new CivicIDOAuthClient(
            //        appId: "",
            //        appSecret: "",
            //        environment: "TEST"),
            //    "Civic ID", null);

            //OAuthWebSecurity.RegisterGoogleClient();
        }
    }
}
