using System;
using System.Security.Claims;
using System.Threading.Tasks;
using SpaceHelmet.Shared.Constants;
using SpaceHelmet.Shared.Support;
using Microsoft.AspNetCore.Components.Authorization;
using TokenClientSupport.Interfaces;
using ClaimValues = SpaceHelmet.Shared.Constants.ClaimValues;

namespace SpaceHelmet.Client.Auth.Support {
    public class AuthStateProvider : AuthenticationStateProvider {
        private readonly ITokenStorageProvider  mTokenProvider;
        private readonly ITokenParser           mTokenParser;
        private readonly AuthenticationState    mAnonymous;

        public AuthStateProvider( ITokenParser tokenParser, ITokenStorageProvider tokenProvider ) {
            mTokenParser = tokenParser;
            mTokenProvider = tokenProvider;

            mAnonymous = new AuthenticationState( new ClaimsPrincipal( new ClaimsIdentity()));
        }

        private AuthenticationState CreateAuthenticationState( string fromToken ) {
            return new AuthenticationState( 
                new ClaimsPrincipal( 
                    new ClaimsIdentity( mTokenParser.GetClaims( fromToken ), JwtConstants.JwtAuthType )));
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync() {
            var token = await mTokenProvider.GetAuthenticationToken();
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
