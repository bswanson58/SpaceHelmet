using System.Security.Claims;
using System.Security.Principal;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using PasetoAuth.Common;
using PasetoAuth.Interfaces;
using PasetoAuth.Options;
using SpaceHelmet.Server.Database.Entities;
using SpaceHelmet.Shared.Constants;
using SpaceHelmet.Shared.Entities;
using SpaceHelmet.Shared.Support;

namespace SpaceHelmet.Server.Auth {
    public class PasetoTokenBuilder : ITokenBuilder {
        private readonly UserManager<DbUser>                    mUserManager;
        private readonly IOptions<PasetoValidationParameters>   mPasetoValidationParameters;
        private readonly IPasetoTokenHandler                    mTokenHandler;

        public PasetoTokenBuilder( IPasetoTokenHandler tokenHandler, UserManager<DbUser> userManager,
                                   IOptions<PasetoValidationParameters> pasetoValidationParameters ) {
            mUserManager = userManager;
            mTokenHandler = tokenHandler;
            mPasetoValidationParameters = pasetoValidationParameters;
        }

        public async Task<WebToken> GenerateToken( DbUser forUser ) {
            var claims = await BuildUserClaims( forUser );
            //var claims = Enumerable.Empty<Claim>();
            var identity = new ClaimsIdentity(
                new GenericIdentity( forUser.Id, "paseto" ),
                claims.Concat( 
                    new [] {
                        new Claim( PasetoRegisteredClaimsNames.TokenIdentifier, Guid.NewGuid().ToString( "N" )),
                    }));

            var expirationTime = DateTimeProvider.Instance.CurrentUtcTime.AddMinutes( 
                Convert.ToDouble( mPasetoValidationParameters.Value.DefaultExpirationTime ));

            var pasetoTokenDescriptor = new PasetoTokenDescriptor() {
                Audience = mPasetoValidationParameters.Value.Audience,
                Expires = expirationTime,
                Issuer = mPasetoValidationParameters.Value.Issuer,
                Subject = identity,
                NotBefore = DateTime.Now
            };

            var publicClaims = await GenerateUserClaims( forUser );
            var paseoToken = await mTokenHandler.WriteTokenAsync( pasetoTokenDescriptor, publicClaims.Serialize());

            return new WebToken( paseoToken );
        }

        public ClaimsPrincipal GetPrincipalFromExpiredToken( string token ) {
            throw new NotImplementedException();
        }

        private async Task<UserClaims> GenerateUserClaims( DbUser forUser ) {
            var expirationTime = DateTimeProvider.Instance.CurrentUtcTime.AddMinutes( 
                Convert.ToDouble( mPasetoValidationParameters.Value.DefaultExpirationTime ));
            var utcTime = new DateTimeOffset( expirationTime.ToUniversalTime());
            var claims = await BuildUserClaims( forUser );

            return new UserClaims( claims.Concat( new [] {
                new Claim( ClaimValues.Expiration, utcTime.ToUnixTimeSeconds().ToString())
            }));
        }

        private async Task<List<Claim>> BuildUserClaims( DbUser user ) {
            var claims = new List<Claim> {
                new( ClaimValues.ClaimEntityId, user.Id ),
                new( ClaimTypes.Email, user.Email ?? String.Empty ),
                new( ClaimValues.ClaimEmailHash, user.Email?.CalculateMd5Hash() ?? String.Empty )
            };

            var dbClaims = await mUserManager.GetClaimsAsync( user );
            claims.AddRange( dbClaims );

            var dbRoles = await mUserManager.GetRolesAsync( user );

            claims.Add(
                dbRoles.Count > 1
                    ? new Claim( ClaimTypes.Role, $"[{String.Join( ",", dbRoles )}]" )
                    : new Claim( ClaimTypes.Role, dbRoles.First() ) );

            return claims;
        }
    }
}
