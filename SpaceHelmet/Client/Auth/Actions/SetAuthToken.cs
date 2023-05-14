using System;

namespace SpaceHelmet.Client.Auth.Actions {
    public class SetAuthToken {
        public  string      Token {  get; }
        public  string      RefreshToken { get; }
        public  DateTime    Expiration { get; }
        public  bool        IsRefresh { get; }

        public SetAuthToken( string token, string refreshToken, DateTime expiration, bool isRefresh ) {
            Token = token;
            RefreshToken = refreshToken;
            Expiration = expiration;
            IsRefresh = isRefresh;
        }
    }

    public class AuthStateUpdated {
        public  bool    WasRefresh { get; }

        public AuthStateUpdated( bool wasRefresh ) {
            WasRefresh = wasRefresh;
        }
    }
}
