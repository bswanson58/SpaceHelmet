using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using SpaceHelmet.Shared.Constants;
using SpaceHelmet.Shared.Support;
using Microsoft.AspNetCore.Components.Authorization;
using SpaceHelmet.Client.Constants;

namespace SpaceHelmet.Client.Auth.Support {
    public class AuthStateProvider : AuthenticationStateProvider {
        private readonly ILocalStorageService   mLocalStorage;
        private readonly ITokenParser           mTokenParser;
        private readonly AuthenticationState    mAnonymous;

        public AuthStateProvider( ILocalStorageService localStorage, ITokenParser tokenParser ) {
            mLocalStorage = localStorage;
            mTokenParser = tokenParser;

            mAnonymous = new AuthenticationState( new ClaimsPrincipal( new ClaimsIdentity()));
        }

        private AuthenticationState CreateAuthenticationState( string fromToken ) {
            return new AuthenticationState( 
                new ClaimsPrincipal( 
                    new ClaimsIdentity( mTokenParser.GetClaims( fromToken ), JwtConstants.JwtAuthType )));
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync() {
            var token = await mLocalStorage.GetItemAsStringAsync( LocalStorageNames.AuthToken );
            var expiration = mTokenParser.GetClaimValue( token, ClaimValues.Expiration );

            if(!String.IsNullOrWhiteSpace( expiration )) {
                var expTime = DateTimeOffset.FromUnixTimeSeconds( Convert.ToInt64( expiration ));
                        
                if(( expTime - DateTimeProvider.Instance.CurrentUtcTime ) > TimeSpan.Zero ) {
                    return CreateAuthenticationState( token );
                }
            }

            return mAnonymous;
        }

        public void SetUserAuthentication() {
            NotifyAuthenticationStateChanged( GetAuthenticationStateAsync());
        }

        public void NotifyUserLogout() {
            var authState = Task.FromResult( mAnonymous );

            NotifyAuthenticationStateChanged( authState );
        }
    }
}
