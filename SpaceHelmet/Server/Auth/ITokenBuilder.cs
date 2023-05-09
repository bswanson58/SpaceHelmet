using SpaceHelmet.Server.Database.Entities;
using System.Security.Claims;
using PasetoAuth.Common;

namespace SpaceHelmet.Server.Auth {
    public class WebToken {
        public string       Token { get; set; }
        public string       RefreshToken { get; set; }
        public DateTime     CreatedAt { get; }
        public DateTime     ExpiresAt { get; set; }

        public WebToken() {
            Token = String.Empty;
            RefreshToken = String.Empty;
            CreatedAt = DateTime.Now;
            ExpiresAt = DateTime.MaxValue;
        }

        public WebToken( PasetoToken fromToken ) {
            Token = fromToken.Token;
            RefreshToken = fromToken.RefreshToken;
            CreatedAt = fromToken.CreatedAt;
            ExpiresAt = fromToken.ExpiresAt;
        }
    }

    public interface ITokenBuilder {
        Task<WebToken>      GenerateToken( DbUser user );
        ClaimsPrincipal     GetPrincipalFromExpiredToken( string token );
    }
}
