using System;
using System.Net.Http;
using Polly;
using Polly.Extensions.Http;

namespace SpaceHelmet.Client.Support {
    public static class RetryPolicyHandler {
        public static IAsyncPolicy<HttpResponseMessage> GetAnonymousRetryPolicy() {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .WaitAndRetryAsync( 3, retryAttempt => TimeSpan.FromMilliseconds( 100 * retryAttempt ));
        }

        public static IAsyncPolicy<HttpResponseMessage> GetAuthorizedRetryPolicy() {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .WaitAndRetryAsync( 3, retryAttempt => TimeSpan.FromMilliseconds( 100 * retryAttempt ));
        }
    }
}
