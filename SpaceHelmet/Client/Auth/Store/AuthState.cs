using System;
using SpaceHelmet.Shared.Support;
using Fluxor;
using SpaceHelmet.Client.Store;

namespace SpaceHelmet.Client.Auth.Store {
    [FeatureState( CreateInitialStateMethodName = "Factory")]
    public class AuthState : RootState {
        public  string      UserToken { get; }
        public  string      RefreshToken { get; }
        public  DateTime    TokenExpiration { get; }

        public AuthState( bool callInProgress, string callMessage,
                          string userToken, string refreshToken, DateTime tokenExpiration ) :
            base( callInProgress, callMessage ) {
            UserToken = userToken;
            RefreshToken = refreshToken;
            TokenExpiration = tokenExpiration;
        }

        public static AuthState Factory() => 
            new( false, string.Empty, String.Empty, String.Empty, DateTimeProvider.Instance.CurrentDateTime );
    }
}
