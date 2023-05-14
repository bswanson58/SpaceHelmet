using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace TokenClientSupport.RefreshTokens {
    internal class NullTokenRefresher : ITokenRefresher {
        public Task<bool> TokenRefreshRequired( int withinMinutes ) =>
            Task.FromResult( false );

        public Task<HttpResponseMessage> RefreshToken() =>
            Task.FromResult( new HttpResponseMessage( HttpStatusCode.OK ));
    }
}
