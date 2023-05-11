using System;

namespace TokenAuthentication.Settings {
    public class RefreshTokenOptions {
        public  bool        UseRefreshTokens { get; set; }
        public  DateTime    RefreshTokenExpiration { get; set; }
    }
}
