using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using SpaceHelmet.Server.Database.Entities;
using TokenAuthentication.Interfaces;
using TokenAuthentication.Models;

namespace SpaceHelmet.Server.Auth {
    public class ClaimBuilder : IClaimBuilder {
        private readonly UserManager<DbUser>    mUserManager;

        public ClaimBuilder( UserManager<DbUser> userManager ) {
            mUserManager = userManager;
        }

        public async Task<IEnumerable<Claim>> GetClaimsAsync( TokenUser user ) {
            if( user is DbUser dbUser ) {
                return await mUserManager.GetClaimsAsync( dbUser );
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
