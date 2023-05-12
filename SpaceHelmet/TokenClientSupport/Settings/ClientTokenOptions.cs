using System;

namespace TokenClientSupport.Settings {
    public class ClientTokenOptions {
        public  string      BaseRoute { get; set; }
        public  bool        UseTokens { get; set; }
        public  bool        UseRefreshTokens { get; set; }
        public  string      TokenStyle { get; set; }

        public ClientTokenOptions() {
            BaseRoute = String.Empty;
            UseTokens = false;
            UseRefreshTokens = false;
            TokenStyle = TokenStyles.JsonWebTokens;
        }
    }
}
