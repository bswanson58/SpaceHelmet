using System;
using Microsoft.AspNetCore.Authentication;

namespace TokenAuthentication.Settings {
    public class PasetoTokenOptions : AuthenticationSchemeOptions {
        public string           SecretKey { get; set; }
        public string ?         Audience { get; set; }
        public string ?         Issuer { get; set; }
        public TimeSpan         TokenExpiration { get; set; }
        public TimeSpan         ClockSkew { get; set; }
        public bool ?           ValidateIssuer { get; set; }
        public bool ?           ValidateAudience { get; set; }

        public PasetoTokenOptions() {
            SecretKey = String.Empty;
            Audience = String.Empty;
            Issuer = String.Empty;
            TokenExpiration = TimeSpan.FromMinutes( 5 );
            ClockSkew = TimeSpan.FromMinutes( 1 );
            ValidateIssuer = false;
            ValidateAudience = false;
        }
    }
}