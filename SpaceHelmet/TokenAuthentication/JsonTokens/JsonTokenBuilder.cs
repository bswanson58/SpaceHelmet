﻿using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TokenAuthentication.Constants;
using TokenAuthentication.Interfaces;
using TokenAuthentication.Models;
using TokenAuthentication.Settings;
using TokenAuthentication.Support;

namespace TokenAuthentication.JsonTokens {
    public class JsonTokenBuilder : ITokenBuilder {
        private readonly IClaimBuilder                  mClaimBuilder;
        private readonly IOptions<JsonTokenOptions>     mJsonTokenOptions;
        private readonly ILogger<JsonTokenBuilder>       mLog;

        public JsonTokenBuilder( IClaimBuilder claimBuilder, IOptions<JsonTokenOptions> jsonTokenOptions, 
                                ILogger<JsonTokenBuilder> log ) {
            mClaimBuilder = claimBuilder;
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
                new( ClaimTypes.Name, user.UserName ?? String.Empty ),
                new( ClaimValues.ClaimEntityId, user.Id ),
                new( ClaimTypes.Email, user.Email ?? String.Empty ),
                new( ClaimValues.ClaimEmailHash, user.Email?.CalculateMd5Hash() ?? String.Empty )
            };

            var dbClaims = await mClaimBuilder.GetClaimsAsync( user );

            claims.AddRange( dbClaims );

            var dbRoles = await mClaimBuilder.GetRolesAsync( user );

            claims.AddRange( dbRoles.Select( r => new Claim( ClaimTypes.Role, r )));

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
            retValue.RefreshToken = mJsonTokenOptions.Value.UseRefreshToken == true ? GenerateRefreshToken() : String.Empty;
            retValue.ExpiresAt = TokenExpiration();

            return retValue;
        }

        private string GenerateRefreshToken() {
            var randomNumber = new byte[32];

            using( var rng = RandomNumberGenerator.Create()) {
                rng.GetBytes( randomNumber );

                return Convert.ToBase64String( randomNumber );
            }
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

        public ClaimsPrincipal GetPrincipalFromExpiredToken( string token ) {
            try {
                var tokenHandler = new JwtSecurityTokenHandler();
                var tokenParameters = CreateTokenValidationParameters();
                var principal = tokenHandler.ValidateToken( token, tokenParameters, out var securityToken );
                var jwtSecurityToken = securityToken as JwtSecurityToken;

                if(( jwtSecurityToken == null ) ||
                   (!jwtSecurityToken.Header.Alg.Equals( SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase ))) {
                    throw new SecurityTokenException( "Invalid token" );
                }

                return principal;
            }
            catch( Exception ex ) {
                mLog.LogError( ex, "GetPrincipalFromExpiredToken" );
            }

            return new ClaimsPrincipal( new ClaimsIdentity( new List<Claim>()));
        }
    }
}