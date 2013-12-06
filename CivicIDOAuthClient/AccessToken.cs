using System.Runtime.Serialization;

namespace Accela.OAuth.Client
{
    [DataContract]
    public class AccessToken
    {
        [DataMember(Name = "access_token")]
        public string Token { get; set; }

        [DataMember(Name = "token_type")]
        public string TokenType { get; set; }

        [DataMember(Name = "expires_in")]
        public int ExpiresInSeconds { get; set; }

        [DataMember(Name = "refresh_token")]
        public string RefreshToken { get; set; }
    }
}