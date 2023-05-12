using System;
using System.Security.Claims;
using System.Threading.Tasks;
using TokenAuthentication.Interfaces;
using TokenClientSupport.Support;

namespace TokenAuthentication.RefreshTokens {
    internal class NullRefreshTokenProvider : IRefreshTokenProvider {
        public Task<string> CreateAsync( ClaimsIdentity claimsPrincipal ) =>
            Task.FromResult( String.Empty );

        public DateTime TokenExpiration() =>
            DateTimeProvider.Instance.CurrentUtcTime;
    }
}
