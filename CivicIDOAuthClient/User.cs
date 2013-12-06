using System.Runtime.Serialization;

namespace Accela.OAuth.Client
{
    [DataContract]
    public class User
    {
        [DataMember(Name = "id")]
        public string Id { get; set; }

        [DataMember(Name = "loginName")]
        public string LoginName { get; set; }

        [DataMember(Name = "email")]
        public string Email { get; set; }

        [DataMember(Name = "firstName")]
        public string FirstName { get; set; }

        [DataMember(Name = "lastName")]
        public string LastName { get; set; }

        [DataMember(Name = "countryCode")]
        public string CountryCode { get; set; }

        [DataMember(Name = "streetAddress")]
        public string StreetAddress { get; set; }

        [DataMember(Name = "city")]
        public string City { get; set; }

        [DataMember(Name = "state")]
        public string State { get; set; }

        [DataMember(Name = "postalCode")]
        public string PostalCode { get; set; }

        [DataMember(Name = "phoneCountryCode")]
        public string PhoneCountryCode { get; set; }

        [DataMember(Name = "phoneAreaCode")]
        public string PhoneAreaCode { get; set; }

        [DataMember(Name = "phoneNumber")]
        public string PhoneNumber { get; set; }

        [DataMember(Name = "avatarUrl")]
        public string AvatarUrl { get; set; }
    }
}