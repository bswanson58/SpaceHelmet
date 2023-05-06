namespace SpaceHelmet.Client.Auth.Actions {
    public class SetAuthToken {
        public  string  Token {  get; }
        public  string  RefreshToken { get; }
        public  bool    IsRefresh { get; }

        public SetAuthToken( string token, string refreshToken, bool isRefresh ) {
            Token = token;
            RefreshToken = refreshToken;
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
