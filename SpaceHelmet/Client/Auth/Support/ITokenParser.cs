using System.Collections.Generic;
using System.Security.Claims;

namespace SpaceHelmet.Client.Auth.Support {
    public interface ITokenParser {
        IEnumerable<Claim>  GetClaims( string token );
        string              GetClaimValue( string token, string claimType );
    }
}
