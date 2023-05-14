using System;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;
using TokenClientSupport.Constants;
using TokenClientSupport.Interfaces;

namespace TokenClientSupport.RefreshTokens {
    public class TokenHandler : DelegatingHandler {
        private readonly ITokenRefresher        mTokenRefresher;
        private readonly ITokenStorageProvider  mTokenProvider;

        public TokenHandler( ITokenRefresher tokenRefresher, ITokenStorageProvider tokenProvider ) {
            mTokenRefresher = tokenRefresher;
            mTokenProvider = tokenProvider;
        }

        protected override async Task<HttpResponseMessage> SendAsync( HttpRequestMessage request, CancellationToken cancellationToken ) {
            if( await mTokenRefresher.TokenRefreshRequired( 2 )) {
                 var response = await mTokenRefresher.RefreshToken();

                 if(!response.IsSuccessStatusCode ) {
                     return response;
                 }
            }

            var authToken = await mTokenProvider.GetAuthenticationToken( cancellationToken );
            var refreshToken = await mTokenProvider.GetRefreshToken( cancellationToken );
            var expiration = await mTokenProvider.GetTokenExpiration( cancellationToken );

            if(!String.IsNullOrWhiteSpace( authToken )) {
                request.Headers.Authorization = new AuthenticationHeaderValue( ClaimValues.AuthScheme, authToken );

                // push our tokens through the request, they will be checked in the response handler to
                // determine if they have been updated. Getting to the AuthFacade is not possible in this context.
                request.Headers.Add( TokenStorageNames.AuthToken, authToken );
                request.Headers.Add( TokenStorageNames.RefreshToken, refreshToken );
                request.Headers.Add( TokenStorageNames.TokenExpiration, expiration.Ticks.ToString());
            }

            return await base.SendAsync( request, cancellationToken );
            /*
            if(( response.StatusCode == HttpStatusCode.Unauthorized ) ||
               ( response.StatusCode == HttpStatusCode.Forbidden )) {
                token = await RefreshTokenAsync();
                request.Headers.Authorization = new AuthenticationHeaderValue( token.Scheme, token.AccessToken );
                response = await base.SendAsync( request, cancellationToken );
            }
            */
        }
    }
}
