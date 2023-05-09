using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.Text;
using SpaceHelmet.Server.Database.Entities;
using SpaceHelmet.Shared.Constants;
using SpaceHelmet.Shared.Support;
using JwtConstants = SpaceHelmet.Shared.Constants.JwtConstants;

namespace SpaceHelmet.Server.Auth {
    public class JwtTokenBuilder : ITokenBuilder {
        private readonly UserManager<DbUser>            mUserManager;
        private readonly IConfigurationSection          mJwtSettings;
        private readonly ILogger<JwtTokenBuilder>       mLog;

        public JwtTokenBuilder( UserManager<DbUser> userManager, IConfiguration configuration, ILogger<JwtTokenBuilder> log ) {
            mUserManager = userManager;
            mLog = log;

            mJwtSettings = configuration.GetSection( JwtConstants.JwtConfigSettings );
        }

        private SigningCredentials GetSigningCredentials() {
            var key = Encoding.UTF8.GetBytes( mJwtSettings[JwtConstants.JwtConfigSecurityKey] ?? String.Empty );
            var secret = new SymmetricSecurityKey( key );

            return new SigningCredentials( secret, SecurityAlgorithms.HmacSha256 );
        }

        private async Task<List<Claim>> BuildUserClaims( DbUser user ) {
            var claims = new List<Claim> {
                new( ClaimTypes.Name, user.UserName ?? String.Empty ),
                new( ClaimValues.ClaimEntityId, user.Id ),
                new( ClaimTypes.Email, user.Email ?? String.Empty ),
                new( ClaimValues.ClaimEmailHash, user.Email?.CalculateMd5Hash() ?? String.Empty )
            };

            var dbClaims = await mUserManager.GetClaimsAsync( user );

            claims.AddRange( dbClaims );

            var dbRoles = await mUserManager.GetRolesAsync( user );

            claims.AddRange( dbRoles.Select( r => new Claim( ClaimTypes.Role, r )));

            return claims;
        }

        private JwtSecurityToken GenerateTokenOptions( SigningCredentials signingCredentials, List<Claim> claims ) {
            var tokenOptions = new JwtSecurityToken(
                issuer: mJwtSettings[JwtConstants.JwtConfigIssuer],
                audience: mJwtSettings[JwtConstants.JwtConfigAudience],
                claims: claims,
                expires: TokenExpiration(),
                signingCredentials: signingCredentials);
            return tokenOptions;
        }

        public async Task<WebToken> GenerateToken( DbUser user ) {
            var retValue = new WebToken();
            var signingCredentials = GetSigningCredentials(); 
            var claims = await BuildUserClaims( user );
            var tokenOptions = GenerateTokenOptions( signingCredentials, claims );

            retValue.Token = new JwtSecurityTokenHandler().WriteToken( tokenOptions );
            retValue.RefreshToken = GenerateRefreshToken();
            retValue.ExpiresAt = TokenExpiration();

            return retValue;
        }

        public string GenerateRefreshToken() {
            var randomNumber = new byte[32];

            using( var rng = RandomNumberGenerator.Create()) {
                rng.GetBytes( randomNumber );

                return Convert.ToBase64String( randomNumber );
            }
        }

        private DateTime TokenExpiration() =>
             DateTimeProvider.Instance.CurrentUtcTime.AddMinutes( 
                 Convert.ToDouble( mJwtSettings[JwtConstants.JwtConfigExpiration]));

        private static TokenValidationParameters CreateTokenValidationParameters( IConfigurationSection jwtSettings ) =>
            new TokenValidationParameters {
                ValidateAudience = false,
                ValidateIssuer = true,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true,
                ValidIssuer = jwtSettings[JwtConstants.JwtConfigIssuer],
                ValidAudience = jwtSettings[JwtConstants.JwtConfigAudience],
                IssuerSigningKey = 
                    new SymmetricSecurityKey( 
                        Encoding.UTF8.GetBytes( jwtSettings[JwtConstants.JwtConfigSecurityKey] ?? String.Empty )),
            };

        public ClaimsPrincipal GetPrincipalFromExpiredToken( string token ) {
            try {
                var tokenHandler = new JwtSecurityTokenHandler();
                var tokenParameters = CreateTokenValidationParameters( mJwtSettings );
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
