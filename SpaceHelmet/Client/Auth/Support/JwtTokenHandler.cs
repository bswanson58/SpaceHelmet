using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using SpaceHelmet.Client.Constants;

namespace SpaceHelmet.Client.Auth.Support {
    public class JwtTokenHandler : DelegatingHandler {
        private readonly ITokenRefresher        mTokenRefresher;
        private readonly ILocalStorageService   mLocalStorage;

        public JwtTokenHandler( ITokenRefresher tokenRefresher, ILocalStorageService storageService ) {
            mTokenRefresher = tokenRefresher;
            mLocalStorage = storageService;
        }

        protected override async Task<HttpResponseMessage> SendAsync( HttpRequestMessage request, CancellationToken cancellationToken ) {
            if( await mTokenRefresher.TokenRefreshRequired( 2 )) {
                 var response = await mTokenRefresher.RefreshToken();

                 if(!response.IsSuccessStatusCode ) {
                     return response;
                 }
            }

            var authToken = await mLocalStorage.GetItemAsStringAsync( LocalStorageNames.AuthToken, cancellationToken );
            var refreshToken = await mLocalStorage.GetItemAsStringAsync( LocalStorageNames.RefreshToken, cancellationToken );

            if(!String.IsNullOrWhiteSpace( authToken )) {
                request.Headers.Authorization = new AuthenticationHeaderValue( "bearer", authToken );

                // push our tokens through the request, they will be checked in the response handler to
                // determine if they have been updated. Getting to the AuthFacade is not possible in this context.
                request.Headers.Add( LocalStorageNames.AuthToken, authToken );
                request.Headers.Add( LocalStorageNames.RefreshToken, refreshToken );
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
