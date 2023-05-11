using System;
using Microsoft.AspNetCore.Authentication;

namespace TokenAuthentication.Settings {
    public class PasetoValidationParameters : AuthenticationSchemeOptions {
        public string           SecretKey { get; set; }
        public string ?         Audience { get; set; }
        public string ?         Issuer { get; set; }
        public int              DefaultExpirationTime { get; set; }
        public TimeSpan         ClockSkew { get; set; }
        public bool ?           ValidateIssuer { get; set; }
        public bool ?           ValidateAudience { get; set; }
        public bool ?           UseRefreshToken { get; set; }

        public PasetoValidationParameters() {
            SecretKey = String.Empty;
            Audience = String.Empty;
            Issuer = String.Empty;
            DefaultExpirationTime = 10;
            ClockSkew = TimeSpan.FromMinutes( 1 );
            ValidateIssuer = false;
            ValidateAudience = false;
            UseRefreshToken = true;
        }
    }
}