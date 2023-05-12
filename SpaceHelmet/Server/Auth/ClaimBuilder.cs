using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using SpaceHelmet.Server.Database.Entities;
using SpaceHelmet.Shared.Constants;
using TokenAuthentication.Interfaces;
using TokenAuthentication.JsonTokens;
using TokenAuthentication.Models;

namespace SpaceHelmet.Server.Auth {
    public class ClaimBuilder : IClaimBuilder {
        private readonly UserManager<DbUser>    mUserManager;

        public ClaimBuilder( UserManager<DbUser> userManager ) {
            mUserManager = userManager;
        }

        public async Task<IEnumerable<Claim>> GetClaimsAsync( TokenUser user ) {
            if( user is DbUser dbUser ) {
                var claims = new List<Claim> {
                    new( ClaimValues.ClaimEntityId, user.Id ),
                    new( ClaimTypes.Email, user.Email ?? String.Empty ),
                    new( ClaimValues.ClaimEmailHash, user.Email?.CalculateMd5Hash() ?? String.Empty )
                };

                var dbClaims = await mUserManager.GetClaimsAsync( dbUser );

                return claims.Concat( dbClaims );
            }

            return Enumerable.Empty<Claim>();
        }

        public async Task<IEnumerable<string>> GetRolesAsync( TokenUser user ) {
            if( user is DbUser dbUser ) {
                return await mUserManager.GetRolesAsync( dbUser );
            }

            return Enumerable.Empty<string>();
        }
    }
}
