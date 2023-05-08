using System;
using Paseto.Builder;
using Paseto;
using SpaceHelmet.Shared.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using SpaceHelmet.Shared.Support;

namespace SpaceHelmet.Client.Auth.Support {
    public class PasetoParser : ITokenParser {
        public IEnumerable<Claim> GetClaims( string token ) {
            if(!String.IsNullOrWhiteSpace( token )) {
                var footerText = new PasetoBuilder()
                    .Use( ProtocolVersion.V4, Purpose.Local )
                    .DecodeFooter( token );

                if(!String.IsNullOrWhiteSpace( footerText )) {
                    var claims = SplitRoleClaim( footerText.Deserialize<UserClaims>());

                    return claims.Select( c => new Claim( c.ClaimType, c.ClaimValue ));
                }
            }

            return Enumerable.Empty<Claim>();
        }

        private IEnumerable<UserClaim> SplitRoleClaim( UserClaims ? claims ) {
            var retValue = new List<UserClaim>();

            if( claims != null ) {
                foreach( var claim in claims.Claims ) {
                    if( claim.ClaimType.Equals( ClaimTypes.Role )) {
                        retValue.AddRange( 
                            SplitRoleClaims( claim.ClaimValue ).Select( c => new UserClaim( c.Type, c.Value )));
                    }
                    else {
                        retValue.Add( claim );
                    }
                }
            }

            return retValue;
        }

        public static IEnumerable<Claim> SplitRoleClaims( string ? claimValues ) {
            if(!String.IsNullOrWhiteSpace( claimValues )) {
                var roles = claimValues.TrimStart( '[' ).TrimEnd( ']' ).Split( ',' );

                return roles.Select( r => new Claim( ClaimTypes.Role, r.Trim( '"' )));
            }

            return Enumerable.Empty<Claim>();
        }

        public string GetClaimValue( string token, string claimType ) {
            var claims = GetClaims( token );
            var claim = claims.FirstOrDefault( c => c.Type.Equals( claimType ));

            return claim != null ? claim.Value : String.Empty;
        }
    }
}
