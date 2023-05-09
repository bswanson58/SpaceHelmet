using System;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using PasetoAuth.Interfaces;

namespace PasetoAuth {
    public class PasetoRefreshTokenProvider : IPasetoRefreshTokenProvider {
        public Task<string> CreateAsync( ClaimsIdentity claimsPrincipal ) {
            var randomNumber = new byte[32];

            using( var rng = RandomNumberGenerator.Create()) {
                rng.GetBytes( randomNumber );

                return Task.FromResult( Convert.ToBase64String( randomNumber ));
            }
        }
    }
}
