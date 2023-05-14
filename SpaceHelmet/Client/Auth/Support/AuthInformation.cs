using System;
using System.Security.Claims;
using SpaceHelmet.Shared.Constants;
using Fluxor;
using SpaceHelmet.Client.Auth.Store;
using SpaceHelmet.Shared.Support;
using TokenClientSupport.Interfaces;

namespace SpaceHelmet.Client.Auth.Support {
    public interface IAuthInformation {
        TimeSpan    TimeOffsetToTokenExpiration { get; }
        bool        IsAuthValid { get; }
        string      UserName { get; }
        string      UserEmail { get; }
        string      UserEmailHash { get; }
        string      UserToken { get; }
        string      RefreshToken { get; }

        bool        IsTokenValid( string jwtToken );
    }

    public class AuthInformation : IAuthInformation {
        private readonly IState<AuthState>      mAuthState;
        private readonly ITokenParser           mTokenParser;

        public AuthInformation( IState<AuthState> authState, ITokenParser tokenParser ) {
            mAuthState = authState;
            mTokenParser = tokenParser;
        }

        public string UserName => 
            IsAuthValid ? 
                mTokenParser.GetClaimValue( mAuthState.Value.UserToken, ClaimTypes.GivenName ) : 
                String.Empty;

        public string UserEmail =>
            IsAuthValid ?
                mTokenParser.GetClaimValue( mAuthState.Value.UserToken, ClaimTypes.Email ) : 
                String.Empty;

        public string UserEmailHash =>
            IsAuthValid ?
                mTokenParser.GetClaimValue( mAuthState.Value.UserToken, ClaimValues.ClaimEmailHash ) : 
                String.Empty;

        public string UserToken =>
            mAuthState.Value.UserToken;

        public string RefreshToken =>
            mAuthState.Value.RefreshToken;

        public bool IsTokenValid( string jwtToken ) =>
            TokenExpirationTime( jwtToken ) > TimeSpan.Zero;

        public bool IsAuthValid =>
            TimeOffsetToTokenExpiration > TimeSpan.Zero;
        
        public TimeSpan TimeOffsetToTokenExpiration =>
            TokenExpirationTime( mAuthState.Value.UserToken );

        private TimeSpan TokenExpirationTime( string jwtToken ) {
            var tokenExpiration = mTokenParser.GetClaimValue( jwtToken, ClaimValues.Expiration );
            var authExpiration = mAuthState.Value.TokenExpiration;

            if(!String.IsNullOrWhiteSpace( tokenExpiration )) {
                var tokenExpirationTime = DateTimeOffset.FromUnixTimeSeconds( Convert.ToInt64( tokenExpiration ));
                var tokenExpirationOffset = tokenExpirationTime - DateTimeProvider.Instance.CurrentUtcTime;
                var authExpirationOffset = authExpiration - DateTimeProvider.Instance.CurrentUtcTime;

                return tokenExpirationOffset > authExpirationOffset ? 
                    tokenExpirationOffset : 
                    authExpirationOffset;
            }

            return TimeSpan.Zero;
        }
    }
}
