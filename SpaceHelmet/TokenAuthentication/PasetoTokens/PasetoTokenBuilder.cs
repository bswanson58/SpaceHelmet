using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using TokenAuthentication.Constants;
using TokenAuthentication.Interfaces;
using TokenAuthentication.Models;
using TokenAuthentication.Settings;
using TokenClientSupport.Constants;
using TokenClientSupport.Support;

namespace TokenAuthentication.PasetoTokens {
    public class PasetoTokenBuilder : ITokenBuilder {
        private readonly IClaimBuilder                  mClaimBuilder;
        private readonly IOptions<PasetoTokenOptions>   mTokenOptions;
        private readonly IPasetoTokenHandler            mTokenHandler;

        public PasetoTokenBuilder( IPasetoTokenHandler tokenHandler, IClaimBuilder claimBuilder,
                                   IOptions<PasetoTokenOptions> tokenOptions ) {
            mClaimBuilder = claimBuilder;
            mTokenHandler = tokenHandler;
            mTokenOptions = tokenOptions;
        }

        public async Task<WebToken> GenerateToken( TokenUser forUser ) {
            var claims = await BuildUserClaims(forUser);
            var identity = new ClaimsIdentity(
                new GenericIdentity( forUser.Id, "paseto" ),
                claims.Concat(
                    new[] {
                        new Claim( PasetoRegisteredClaimsNames.TokenIdentifier, Guid.NewGuid().ToString( "N" )),
                    }));

            var expirationTime = DateTimeProvider.Instance.CurrentUtcTime + mTokenOptions.Value.TokenExpiration;

            var pasetoTokenDescriptor = new PasetoTokenDescriptor {
                Audience = mTokenOptions.Value.Audience,
                Expires = expirationTime,
                Issuer = mTokenOptions.Value.Issuer,
                Subject = identity,
                NotBefore = DateTime.Now
            };

            var publicClaims = await GenerateUserClaims( forUser );

            return await mTokenHandler.WriteTokenAsync( pasetoTokenDescriptor, publicClaims.Serialize() );
        }

        public async Task<ClaimsPrincipal> GetPrincipalFromExpiredToken( string token ) =>
            await mTokenHandler.DecodeTokenAsync( token );

        private async Task<UserClaims> GenerateUserClaims( TokenUser forUser ) {
            var utcTime = new DateTimeOffset( TokenExpiration().ToUniversalTime());
            var claims = await BuildUserClaims( forUser );

            return new UserClaims( claims.Concat( new[] {
                new Claim( ClaimValues.Expiration, utcTime.ToUnixTimeSeconds().ToString())
            }));
        }

        private async Task<List<Claim>> BuildUserClaims( TokenUser user ) {
            var claims = new List<Claim> {
                new( ClaimValues.RefreshName, user.UserName ?? String.Empty ),
            };

            claims.AddRange( await mClaimBuilder.GetClaimsAsync( user ));

            var dbRoles = ( await mClaimBuilder.GetRolesAsync( user )).ToList();

            claims.Add(
                dbRoles.Count > 1
                    ? new Claim( ClaimTypes.Role, $"[{string.Join( ",", dbRoles )}]" )
                    : new Claim( ClaimTypes.Role, dbRoles.First()));

            return claims;
        }

        private DateTime TokenExpiration() =>
            DateTimeProvider.Instance.CurrentUtcTime + mTokenOptions.Value.TokenExpiration; 
    }
}
