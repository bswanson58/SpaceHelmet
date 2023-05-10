namespace SpaceHelmet.Server.Auth.Settings {
    public static class TokenStyles {
        public const string   JsonWebTokens = "jwt";
        public const string   PasetoTokens = "paseto";
    }

    public class TokenOptions {
        public  bool        UseTokens { get; set; }
        public  string      TokenStyle { get; set; }
        public  TimeSpan    TokenExpiration { get; set; }

        public TokenOptions() {
            UseTokens = false;
            TokenStyle = TokenStyles.JsonWebTokens;
            TokenExpiration = TimeSpan.FromMinutes( 60 );
        }
    }
}
