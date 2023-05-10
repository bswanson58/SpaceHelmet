using System;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using TokenAuthentication.Interfaces;

namespace TokenAuthentication.RefreshTokens {
    public class RefreshTokenProvider : IRefreshTokenProvider {
        public Task<string> CreateAsync( ClaimsIdentity claimsPrincipal ) {
            var randomNumber = new byte[32];

            using( var rng = RandomNumberGenerator.Create()) {
                rng.GetBytes( randomNumber );

                return Task.FromResult( Convert.ToBase64String( randomNumber ));
            }
        }
    }
}
