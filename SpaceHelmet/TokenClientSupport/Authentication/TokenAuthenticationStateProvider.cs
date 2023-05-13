using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Authorization;
using TokenClientSupport.Constants;
using TokenClientSupport.Interfaces;
using TokenClientSupport.Support;

namespace TokenClientSupport.Authentication {
    public class TokenAuthenticationStateProvider : AuthenticationStateProvider {
        private readonly ITokenStorageProvider  mTokenProvider;
        private readonly ITokenParser           mTokenParser;
        private readonly AuthenticationState    mAnonymous;

        public TokenAuthenticationStateProvider( ITokenParser tokenParser, ITokenStorageProvider tokenProvider ) {
            mTokenParser = tokenParser;
            mTokenProvider = tokenProvider;

            mAnonymous = new AuthenticationState( new ClaimsPrincipal( new ClaimsIdentity()));
        }

        private AuthenticationState CreateAuthenticationState( string fromToken ) {
            return new AuthenticationState( 
                new ClaimsPrincipal( 
                    new ClaimsIdentity( mTokenParser.GetClaims( fromToken ), ClaimValues.AuthType )));
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
