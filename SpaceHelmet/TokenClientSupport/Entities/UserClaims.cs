using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Text.Json.Serialization;

namespace TokenClientSupport.Entities {
    [DebuggerDisplay("Claim: {" + nameof( ClaimType ) + "}")]
    public class UserClaim {
        public  string  ClaimType { get; set; }
        public  string  ClaimValue { get; set; }

        [JsonConstructor]
        public UserClaim( string claimType, string claimValue ) {
            ClaimType = claimType;
            ClaimValue = claimValue;
        }

        public UserClaim() {
            ClaimType = String.Empty;
            ClaimValue = String.Empty;
        }

        public UserClaim( Claim claim ) {
            ClaimType = claim.Type;
            ClaimValue = claim.Value;
        }
    }

    public class UserClaims {
        public  List<UserClaim> Claims { get; set; }

        [JsonConstructor]
        public UserClaims( List<UserClaim> claims ) {
            Claims = claims;
        }

        public UserClaims() {
            Claims = new List<UserClaim>();
        }

        public UserClaims( IEnumerable<Claim> claims ) {
            Claims = new List<UserClaim>( claims.Select( c => new UserClaim( c )));
        }
    }
}
