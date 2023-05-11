using Microsoft.AspNetCore.Authentication;
using System;

namespace TokenAuthentication.Settings {
    public class JsonTokenOptions : AuthenticationSchemeOptions {
        public string           SecretKey { get; set; }
        public string ?         Audience { get; set; }
        public string ?         Issuer { get; set; }
        public TimeSpan         TokenExpiration { get; set; }
        public bool ?           ValidateIssuer { get; set; }
        public bool ?           ValidateAudience { get; set; }

        public JsonTokenOptions() {
            SecretKey = String.Empty;
            Audience = String.Empty;
            Issuer = String.Empty;
            TokenExpiration = TimeSpan.FromMinutes( 5 );
            ValidateIssuer = false;
            ValidateAudience = false;
        }
    }
}
