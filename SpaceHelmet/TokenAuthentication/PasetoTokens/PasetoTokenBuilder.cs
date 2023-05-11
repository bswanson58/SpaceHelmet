using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using TokenAuthentication.Constants;
using TokenAuthentication.Interfaces;
using TokenAuthentication.JsonTokens;
using TokenAuthentication.Models;
using TokenAuthentication.Settings;
using TokenAuthentication.Support;

namespace TokenAuthentication.PasetoTokens {
    public class PasetoTokenBuilder : ITokenBuilder {
        private readonly IClaimBuilder                          mClaimBuilder;
        private readonly IOptions<PasetoValidationParameters>   mPasetoValidationParameters;
        private readonly IPasetoTokenHandler                    mTokenHandler;

        public PasetoTokenBuilder( IPasetoTokenHandler tokenHandler, IClaimBuilder claimBuilder,
                                   IOptions<PasetoValidationParameters> pasetoValidationParameters ) {
            mClaimBuilder = claimBuilder;
            mTokenHandler = tokenHandler;
            mPasetoValidationParameters = pasetoValidationParameters;
        }

        public async Task<WebToken> GenerateToken( TokenUser forUser ) {
            var claims = await BuildUserClaims(forUser);
            //var claims = Enumerable.Empty<Claim>();
            var identity = new ClaimsIdentity(
                new GenericIdentity(forUser.Id, "paseto"),
                claims.Concat(
                    new[] {
                        new Claim( PasetoRegisteredClaimsNames.TokenIdentifier, Guid.NewGuid().ToString( "N" )),
                    }));

            var expirationTime = DateTimeProvider.Instance.CurrentUtcTime.AddMinutes(
                Convert.ToDouble(mPasetoValidationParameters.Value.DefaultExpirationTime));

            var pasetoTokenDescriptor = new PasetoTokenDescriptor() {
                Audience = mPasetoValidationParameters.Value.Audience,
                Expires = expirationTime,
                Issuer = mPasetoValidationParameters.Value.Issuer,
                Subject = identity,
                NotBefore = DateTime.Now
            };

            var publicClaims = await GenerateUserClaims(forUser);
            return await mTokenHandler.WriteTokenAsync(pasetoTokenDescriptor, publicClaims.Serialize());
        }

        public ClaimsPrincipal GetPrincipalFromExpiredToken( string token ) {
            throw new NotImplementedException();
        }

        private async Task<UserClaims> GenerateUserClaims( TokenUser forUser ) {
            var expirationTime = DateTimeProvider.Instance.CurrentUtcTime.AddMinutes(
                Convert.ToDouble(mPasetoValidationParameters.Value.DefaultExpirationTime));
            var utcTime = new DateTimeOffset(expirationTime.ToUniversalTime());
            var claims = await BuildUserClaims(forUser);

            return new UserClaims( claims.Concat( new[] {
                new Claim( ClaimValues.Expiration, utcTime.ToUnixTimeSeconds().ToString())
            } ) );
        }

        private async Task<List<Claim>> BuildUserClaims( TokenUser user ) {
            var claims = new List<Claim> {
                new( ClaimValues.ClaimEntityId, user.Id ),
                new( ClaimTypes.Email, user.Email ?? string.Empty ),
                new( ClaimValues.ClaimEmailHash, user.Email?.CalculateMd5Hash() ?? string.Empty )
            };

            claims.AddRange( await mClaimBuilder.GetClaimsAsync( user ));

            var dbRoles = ( await mClaimBuilder.GetRolesAsync( user )).ToList();

            claims.Add(
                dbRoles.Count > 1
                    ? new Claim( ClaimTypes.Role, $"[{string.Join( ",", dbRoles )}]" )
                    : new Claim( ClaimTypes.Role, dbRoles.First() ) );

            return claims;
        }
    }
}
