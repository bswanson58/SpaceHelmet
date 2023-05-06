using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using SpaceHelmet.Shared.Dto.Auth;
using SpaceHelmet.Shared.Support;
using Microsoft.Extensions.Logging;
using SpaceHelmet.Client.Constants;

namespace SpaceHelmet.Client.Auth.Support {
    public interface ITokenRefresher {
        Task<bool>                  TokenRefreshRequired( int withinMinutes );
        Task<HttpResponseMessage>   RefreshToken();
    }

    public class JwtTokenRefresher : ITokenRefresher {
        private readonly IHttpClientFactory             mClientFactory;
        private readonly ILocalStorageService           mLocalStorage;
        private readonly ILogger<JwtTokenRefresher>     mLog;

        public JwtTokenRefresher( IHttpClientFactory clientFactory, ILocalStorageService localStorage,
                                  ILogger<JwtTokenRefresher> log ) {
            mClientFactory = clientFactory;
            mLocalStorage = localStorage;
            mLog = log;
        }

        private async Task<DateTimeOffset> TokenExpirationTime() {
            var token = await mLocalStorage.GetItemAsStringAsync( LocalStorageNames.AuthToken );
            var expiration = JwtParser.GetClaimValue( token, "exp" );

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
                var authToken = await mLocalStorage.GetItemAsStringAsync( LocalStorageNames.AuthToken );
                var refreshToken = await mLocalStorage.GetItemAsStringAsync( LocalStorageNames.RefreshToken );

                if((!String.IsNullOrWhiteSpace( authToken )) &&
                   (!String.IsNullOrWhiteSpace( refreshToken ))) {
                    var request = new RefreshTokenRequest {
                        Token = authToken,
                        RefreshToken = refreshToken
                    };

                    using var httpClient = mClientFactory.CreateClient( HttpClientNames.Anonymous );
                    var postResponse = await httpClient.PostAsJsonAsync( RefreshTokenRequest.Route, request );

                    if( postResponse.IsSuccessStatusCode ) {
                        var response = await postResponse.Content.ReadFromJsonAsync<RefreshTokenResponse>();

                        if( response?.Succeeded == true ) {
                            await mLocalStorage.SetItemAsStringAsync( LocalStorageNames.AuthToken, response.Token );
                            await mLocalStorage.SetItemAsStringAsync( LocalStorageNames.RefreshToken, response.RefreshToken );
                        }
                    }

                    return postResponse;
                }
            }
            catch( Exception ex ) {
                mLog.LogError( ex, "Attempting to refresh JWT token" );
            }

            return new HttpResponseMessage( HttpStatusCode.Unauthorized );
        }
    }
}
