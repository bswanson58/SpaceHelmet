using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TokenAuthentication.Interfaces;
using TokenAuthentication.Models;
using TokenAuthentication.Settings;
using TokenClientSupport.Constants;
using TokenClientSupport.Support;

namespace TokenAuthentication.JsonTokens {
    public class JsonTokenBuilder : ITokenBuilder {
        private readonly IClaimBuilder                  mClaimBuilder;
        private readonly IRefreshTokenProvider          mRefreshTokenProvider;
        private readonly IOptions<JsonTokenOptions>     mJsonTokenOptions;
        private readonly ILogger<JsonTokenBuilder>       mLog;

        public JsonTokenBuilder( IClaimBuilder claimBuilder, IRefreshTokenProvider refreshTokenProvider,
                                 IOptions<JsonTokenOptions> jsonTokenOptions, ILogger<JsonTokenBuilder> log ) {
            mClaimBuilder = claimBuilder;
            mRefreshTokenProvider = refreshTokenProvider;
            mJsonTokenOptions = jsonTokenOptions;
            mLog = log;
        }

        private SigningCredentials GetSigningCredentials() {
            var key = Encoding.UTF8.GetBytes( mJsonTokenOptions.Value.SecretKey );
            var secret = new SymmetricSecurityKey( key );

            return new SigningCredentials( secret, SecurityAlgorithms.HmacSha256 );
        }

        private async Task<List<Claim>> BuildUserClaims( TokenUser user ) {
            var claims = new List<Claim> {
                new( ClaimValues.RefreshName, user.UserName ?? String.Empty ),
            };

            var dbClaims = await mClaimBuilder.GetClaimsAsync( user );

            claims.AddRange( dbClaims );

            var dbRoles = ( await mClaimBuilder.GetRolesAsync( user )).ToList();

            claims.Add(
                dbRoles.Count > 1
                    ? new Claim( ClaimTypes.Role, $"[{string.Join( ",", dbRoles )}]" )
                    : new Claim( ClaimTypes.Role, dbRoles.First()));

            return claims;
        }

        private JwtSecurityToken GenerateTokenOptions( SigningCredentials signingCredentials, List<Claim> claims ) {
            var tokenOptions = new JwtSecurityToken(
                issuer: mJsonTokenOptions.Value.Issuer,
                audience: mJsonTokenOptions.Value.Audience,
                claims: claims,
                expires: TokenExpiration(),
                signingCredentials: signingCredentials);
            return tokenOptions;
        }

        public async Task<WebToken> GenerateToken( TokenUser user ) {
            var retValue = new WebToken();
            var signingCredentials = GetSigningCredentials(); 
            var claims = await BuildUserClaims( user );
            var tokenOptions = GenerateTokenOptions( signingCredentials, claims );

            retValue.Token = new JwtSecurityTokenHandler().WriteToken( tokenOptions );
            retValue.RefreshToken = await mRefreshTokenProvider.CreateAsync( new ClaimsIdentity( claims ));
            retValue.ExpiresAt = TokenExpiration();
            retValue.RefreshExpiresAt = mRefreshTokenProvider.TokenExpiration();

            return retValue;
        }

        private DateTime TokenExpiration() =>
             DateTimeProvider.Instance.CurrentUtcTime + mJsonTokenOptions.Value.TokenExpiration; 

        private TokenValidationParameters CreateTokenValidationParameters() =>
            new TokenValidationParameters {
                ValidAudience = mJsonTokenOptions.Value.Audience,
                ValidateAudience = mJsonTokenOptions.Value.ValidateAudience ?? false,
                ValidIssuer = mJsonTokenOptions.Value.Issuer,
                ValidateIssuer = mJsonTokenOptions.Value.ValidateIssuer ?? false,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true,
                IssuerSigningKey = 
                    new SymmetricSecurityKey( 
                        Encoding.UTF8.GetBytes( mJsonTokenOptions.Value.SecretKey )),
            };

        public Task<ClaimsPrincipal> GetPrincipalFromExpiredToken( string token ) {
            try {
                var tokenHandler = new JwtSecurityTokenHandler();
                var tokenParameters = CreateTokenValidationParameters();
                var principal = tokenHandler.ValidateToken( token, tokenParameters, out var securityToken );
                var jwtSecurityToken = securityToken as JwtSecurityToken;

                if(( jwtSecurityToken == null ) ||
                   (!jwtSecurityToken.Header.Alg.Equals( SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase ))) {
                    throw new SecurityTokenException( "Invalid token" );
                }

                return Task.FromResult( principal );
            }
            catch( Exception ex ) {
                mLog.LogError( ex, "GetPrincipalFromExpiredToken" );
            }

            return Task.FromResult( new ClaimsPrincipal( new ClaimsIdentity( new List<Claim>())));
        }
    }
}
