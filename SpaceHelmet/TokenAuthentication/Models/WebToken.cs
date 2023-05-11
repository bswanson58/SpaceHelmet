using System;

namespace TokenAuthentication.Models {
    public class WebToken {
        public string       Token { get; set; }
        public string       RefreshToken { get; set; }
        public DateTime     CreatedAt { get; }
        public DateTime     ExpiresAt { get; set; }
        public DateTime     RefreshExpiresAt { get; set; }

        public WebToken() {
            Token = String.Empty;
            RefreshToken = String.Empty;
            CreatedAt = DateTime.Now;
            ExpiresAt = DateTime.MaxValue;
            RefreshExpiresAt = DateTime.MaxValue;
        }
    }
}
