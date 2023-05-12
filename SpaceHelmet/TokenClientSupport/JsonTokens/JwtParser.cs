using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using TokenClientSupport.Interfaces;

namespace TokenClientSupport.JsonTokens {
    public class JwtParser : ITokenParser {
        public IEnumerable<Claim>  GetClaims( string token ) => 
            JwtStaticParser.GetClaims( token );

        public string GetClaimValue( string token, string claimType ) => 
            JwtStaticParser.GetClaimValue( token, claimType );
    }

    internal static class JwtStaticParser {
        public static IEnumerable<Claim> GetClaims( string jwt ) {
            var retValue = new List<Claim>();
            var claims = ParseClaimsFromJwt( jwt );

            retValue.AddRange( ParseNonRoleClaims( claims ));
            retValue.AddRange( ParseRoleClaims( claims ));

            return retValue;
        }

        private static IList<Claim> ParseClaimsFromJwt( string jwt ) {
            var claims = new List<Claim>();

            if(!String.IsNullOrWhiteSpace( jwt )) {
                var payload = jwt.Split ('.' )[1];
                var jsonBytes = ParseBase64WithoutPadding( payload );
                var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>( jsonBytes );

                if( keyValuePairs != null ) {
                    claims.AddRange( 
                        keyValuePairs
                            .Select( kvp => new Claim( kvp.Key, kvp.Value.ToString() ?? String.Empty )));
                }
            }

            return claims;
        }

        private static IEnumerable<Claim> ParseNonRoleClaims( IEnumerable<Claim> claims ) =>
            claims.Where( c => !c.Type.Equals( ClaimTypes.Role ));

        private static IList<Claim> ParseRoleClaims( IEnumerable<Claim> claims ) {
            var retValue = new List<Claim>();

            foreach( var c in claims ) {
                if( c.Type.Equals( ClaimTypes.Role )) {
                    var roles = c.Value.TrimStart( '[' ).TrimEnd( ']' ).Split( ',' );

                    retValue.AddRange( roles.Select( r => new Claim( ClaimTypes.Role, r.Trim( '"' ))));
                }
            } 
            return retValue;
        }
/*
        private static IList<Claim> ParseRolesFromJwt( string jwt ) {
            var retValue = new List<Claim>();
            var roleValues = ParseClaimsFromJwt( jwt )
                .Where( c => c.Type.Equals( ClaimTypes.Role ))
                .Select( c => c.Value )
                .ToList();

            foreach( var roleValue in roleValues ) {
                var roles = roleValue.TrimStart( '[' ).TrimEnd( ']' ).Split( ',' );

                retValue.AddRange( roles.Select( r => new Claim( ClaimTypes.Role, r.Trim( '"' ))));
            }

            return retValue;
        }
*/
        public static string GetClaimValue( string jwt, string claimType ) =>
            ParseClaimsFromJwt( jwt )
                .FirstOrDefault( c => c.Type.Equals( claimType ), new Claim( String.Empty, String.Empty ))
                .Value;

        private static byte[] ParseBase64WithoutPadding( string base64 ) {
            switch( base64.Length % 4 ) {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }

            return Convert.FromBase64String( base64 );
        }
    }
}
