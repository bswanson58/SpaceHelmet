using System;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using TokenAuthentication.Interfaces;
using TokenAuthentication.Settings;
using TokenClientSupport.Support;

namespace TokenAuthentication.RefreshTokens {
    public class RefreshTokenProvider : IRefreshTokenProvider {
        private readonly IOptions<RefreshTokenOptions>  mTokenOptions;

        public RefreshTokenProvider( IOptions<RefreshTokenOptions> tokenOptions ) {
            mTokenOptions = tokenOptions;
        }

        public Task<string> CreateAsync( ClaimsIdentity claimsPrincipal ) {
            var randomNumber = new byte[32];

            using( var rng = RandomNumberGenerator.Create()) {
                rng.GetBytes( randomNumber );

                return Task.FromResult( Convert.ToBase64String( randomNumber ));
            }
        }

        public DateTime TokenExpiration() =>
            DateTimeProvider.Instance.CurrentUtcTime + mTokenOptions.Value.RefreshTokenExpiration;
    }
}
