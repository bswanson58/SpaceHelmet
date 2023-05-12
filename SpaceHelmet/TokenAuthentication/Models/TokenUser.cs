using System;
using Microsoft.AspNetCore.Identity;
using TokenClientSupport.Support;

namespace TokenAuthentication.Models {
    public class TokenUser : IdentityUser {
        public  string      RefreshToken { get; set; }
        public  DateTime    RefreshTokenExpiration { get; set; }

        public TokenUser() {
            RefreshToken = String.Empty;
            RefreshTokenExpiration = DateTimeProvider.Instance.CurrentDateTime;
        }
    }
}