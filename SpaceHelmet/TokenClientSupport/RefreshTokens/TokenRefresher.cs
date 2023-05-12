using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using System;
using System.Net.Http.Json;
using Blazored.LocalStorage;
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
        private readonly ILocalStorageService       mLocalStorage;
        private readonly ILogger<TokenRefresher>    mLog;

        public TokenRefresher( IHttpClientFactory clientFactory, ILocalStorageService localStorage,
                               ITokenParser tokenParser, ILogger<TokenRefresher> log ) {
            mClientFactory = clientFactory;
            mLocalStorage = localStorage;
            mTokenParser = tokenParser;
            mLog = log;
        }

        private async Task<DateTimeOffset> TokenExpirationTime() {
            var token = await mLocalStorage.GetItemAsStringAsync( TokenStorageNames.AuthToken );
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
                var authToken = await mLocalStorage.GetItemAsStringAsync( TokenStorageNames.AuthToken );
                var refreshToken = await mLocalStorage.GetItemAsStringAsync( TokenStorageNames.RefreshToken );

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
                            await mLocalStorage.SetItemAsStringAsync( TokenStorageNames.AuthToken, response.Token );
                            await mLocalStorage.SetItemAsStringAsync( TokenStorageNames.RefreshToken, response.RefreshToken );
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
