﻿using System;
using TokenClientSupport.Settings;

namespace TokenAuthentication.Settings {

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