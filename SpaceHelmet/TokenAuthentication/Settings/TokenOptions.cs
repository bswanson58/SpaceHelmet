using System;

namespace TokenAuthentication.Settings {
    public static class TokenStyles {
        public const string   JsonWebTokens = "jwt";
        public const string   PasetoTokens = "paseto";
    }

    public class TokenOptions {
        public  bool        UseTokens { get; set; }
        public  bool        UseRefreshTokens { get; set; }
        public  string      TokenStyle { get; set; }

        public TokenOptions() {
            UseTokens = false;
            UseRefreshTokens = false;
            TokenStyle = TokenStyles.JsonWebTokens;
        }
    }
}
