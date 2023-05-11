using System;

namespace TokenAuthentication.Settings {
    public class RefreshTokenOptions {
        public  TimeSpan    RefreshTokenExpiration { get; set; }

        public RefreshTokenOptions() {
            RefreshTokenExpiration = TimeSpan.FromMinutes( 60 );
        }
    }
}
