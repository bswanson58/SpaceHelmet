using Microsoft.AspNetCore.Identity;
using SpaceHelmet.Shared.Support;

namespace SpaceHelmet.Server.Database.Entities {
    public class DbUser : IdentityUser {
        public  string      RefreshToken { get; set; }
        public  DateTime    RefreshTokenExpiration { get; set; }

        public DbUser() {
            RefreshToken = String.Empty;
            RefreshTokenExpiration = DateTimeProvider.Instance.CurrentDateTime;
        }
    }
}
