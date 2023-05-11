using System;
using System.Security.Claims;

namespace TokenAuthentication.Settings {
    public class PasetoTokenDescriptor {
        public string ?         Issuer { get; set; }
        public string ?         Audience { get; set; }
        public ClaimsIdentity   Subject { get; set; }
        public DateTime ?       NotBefore { get; set; }
        public DateTime ?       Expires { get; set; }

        public PasetoTokenDescriptor() {
            Issuer = String.Empty;
            Audience = String.Empty;
            Subject = new ClaimsIdentity();
            NotBefore = DateTime.MinValue;
            Expires = DateTime.MaxValue;
        }
    }
}