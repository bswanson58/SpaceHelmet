using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using SpaceHelmet.Server.Database.Entities;
using SpaceHelmet.Shared.Constants;
using SpaceHelmet.Shared.Support;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using Microsoft.Extensions.Options;
using PasetoAuth.Common;
using PasetoAuth.Interfaces;
using PasetoAuth.Options;
using SpaceHelmet.Shared.Entities;
using JwtConstants = SpaceHelmet.Shared.Constants.JwtConstants;

namespace SpaceHelmet.Server.Auth {
    public interface ITokenBuilder {
        Task<PasetoToken>   GenerateToken( DbUser user );
        string              GenerateRefreshToken();
        DateTime            TokenExpiration();
        ClaimsPrincipal     GetPrincipalFromExpiredToken( string token );
    }

    public class TokenBuilder : ITokenBuilder {
        private readonly UserManager<DbUser>            mUserManager;
        private readonly IConfigurationSection          mJwtSettings;
        private readonly IOptions<PasetoValidationParameters>   mPasetoValidationParameters;
        private readonly IPasetoTokenHandler            mTokenHandler;
        private readonly ILogger<TokenBuilder>          mLog;

        public TokenBuilder( UserManager<DbUser> userManager, IConfiguration configuration, ILogger<TokenBuilder> log, 
                             IPasetoTokenHandler tokenHandler, 
                             IOptions<PasetoValidationParameters> validationParameters ) {
            mUserManager = userManager;
            mLog = log;
            mTokenHandler = tokenHandler;

            mJwtSettings = configuration.GetSection( JwtConstants.JwtConfigSettings );
            mPasetoValidationParameters = validationParameters;
        }

        private SigningCredentials GetSigningCredentials() {
            var key = Encoding.UTF8.GetBytes( mJwtSettings[JwtConstants.JwtConfigSecurityKey] ?? String.Empty );
            var secret = new SymmetricSecurityKey( key );

            return new SigningCredentials( secret, SecurityAlgorithms.HmacSha256 );
        }

        private async Task<List<Claim>> BuildUserClaims( DbUser user ) {
            var claims = new List<Claim> {
//                new( ClaimTypes.Name, user.UserName ?? String.Empty ),
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
//            claims.AddRange( dbRoles.Select( r => new Claim( ClaimTypes.Role, r )));

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

        public async Task<PasetoToken> GenerateToken( DbUser user ) {
//            var signingCredentials = GetSigningCredentials(); 
//            var claims = await BuildUserClaims( user );
//            var tokenOptions = GenerateTokenOptions( signingCredentials, claims );

            return await GeneratePasetoToken( user );

//            return new JwtSecurityTokenHandler().WriteToken( tokenOptions );
        }

        public string GenerateRefreshToken() {
            var randomNumber = new byte[32];

            using( var rng = RandomNumberGenerator.Create()) {
                rng.GetBytes( randomNumber );

                return Convert.ToBase64String( randomNumber );
            }
        }

        public DateTime TokenExpiration() =>
             DateTimeProvider.Instance.CurrentUtcTime.AddMinutes( 
                 Convert.ToDouble( mJwtSettings[JwtConstants.JwtConfigExpiration]));

        public static TokenValidationParameters CreateTokenValidationParameters( IConfigurationSection jwtSettings ) =>
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

        private async Task<PasetoToken> GeneratePasetoToken( DbUser forUser ) {
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

            return await mTokenHandler.WriteTokenAsync( pasetoTokenDescriptor, publicClaims.Serialize());
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
    }
}
