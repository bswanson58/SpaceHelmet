using System;
using System.Net.Http;
using Polly;
using Polly.Extensions.Http;

namespace TokenClientSupport.RefreshTokens {
    internal static class RefreshPolicyHandler {
        public static IAsyncPolicy<HttpResponseMessage> GetRefreshRetryPolicy() {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .WaitAndRetryAsync( 3, retryAttempt => TimeSpan.FromMilliseconds( 100 * retryAttempt ));
        }
    }
}
