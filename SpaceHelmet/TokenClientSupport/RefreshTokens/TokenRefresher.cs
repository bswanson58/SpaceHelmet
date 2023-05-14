using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using System;
using System.Net.Http.Json;
using TokenClientSupport.Constants;
using TokenClientSupport.Dto;
using TokenClientSupport.Interfaces;
using TokenClientSupport.Support;

namespace TokenClientSupport.RefreshTokens {
    public interface ITokenRefresher {
        Task<bool>                  TokenRefreshRequired( int withinMinutes );
        Task<HttpResponseMessage>   RefreshToken();
    }

    public class TokenRefresher : ITokenRefresher {
        private readonly IHttpClientFactory         mClientFactory;
        private readonly ITokenParser               mTokenParser;
        private readonly ITokenStorageProvider      mTokenProvider;
        private readonly ILogger<TokenRefresher>    mLog;

        public TokenRefresher( IHttpClientFactory clientFactory, ITokenStorageProvider tokenProvider,
                               ITokenParser tokenParser, ILogger<TokenRefresher> log ) {
            mClientFactory = clientFactory;
            mTokenProvider = tokenProvider;
            mTokenParser = tokenParser;
            mLog = log;
        }

        private async Task<DateTimeOffset> TokenExpirationTime() {
            var token = await mTokenProvider.GetAuthenticationToken();
            var expiration = mTokenParser.GetClaimValue( token, ClaimValues.Expiration );

            if(!String.IsNullOrWhiteSpace( expiration )) {
                return DateTimeOffset.FromUnixTimeSeconds( Convert.ToInt64( expiration ));
            }

            return DateTimeOffset.MinValue;
        }

        public async Task<bool> TokenRefreshRequired( int withinMinutes ) {
            var expTime = await TokenExpirationTime();
            var diff = expTime - DateTimeProvider.Instance.CurrentUtcTime;

            return diff.TotalMinutes <= withinMinutes;
        }

        public async Task<HttpResponseMessage> RefreshToken() {
            try {
                var authToken = await mTokenProvider.GetAuthenticationToken();
                var refreshToken = await mTokenProvider.GetRefreshToken();

                if((!String.IsNullOrWhiteSpace( authToken )) &&
                   (!String.IsNullOrWhiteSpace( refreshToken ))) {
                    var request = new RefreshTokenRequest {
                        Token = authToken,
                        RefreshToken = refreshToken
                    };

                    using var httpClient = mClientFactory.CreateClient( TokenClientNames.RefreshClient );
                    var postResponse = await httpClient.PostAsJsonAsync( RefreshTokenRequest.Route, request );

                    if( postResponse.IsSuccessStatusCode ) {
                        var response = await postResponse.Content.ReadFromJsonAsync<RefreshTokenResponse>();

                        if( response?.Succeeded == true ) {
                            await mTokenProvider.StoreAuthenticationToken( response.Token );
                            await mTokenProvider.StoreRefreshToken( response.RefreshToken );
                            await mTokenProvider.StoreTokenExpiration( response.TokenExpiration );
                        }
                    }

                    return postResponse;
                }
            }
            catch( Exception ex ) {
                mLog.LogError( ex, "Attempting to refresh the authentication token" );
            }

            return new HttpResponseMessage( HttpStatusCode.Unauthorized );
        }
    }
}
