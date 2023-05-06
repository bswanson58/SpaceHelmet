using System.Diagnostics;
using System.Text.Json.Serialization;

namespace SpaceHelmet.Shared.Entities {
    [DebuggerDisplay("User: {" + nameof( LoginName ) + "}")]
    public class ShUser : EntityBase {
        public  string          DisplayName { get; }
        public  string          LoginName { get; }
        public  string          Email { get; }
        public  IList<string>   RoleClaims { get; }

        [JsonConstructor]
        public ShUser( string entityId, string loginName, string displayName, string email, IList<string> roleClaims ) :
            base( entityId, entityId ){
            DisplayName = displayName;
            LoginName = loginName;
            Email = email;
            RoleClaims = roleClaims;
        }

        public ShUser With( string ?  displayName, string ? email = null ) {
            return new ShUser( EntityId, LoginName, displayName ?? DisplayName, email ?? Email, RoleClaims );
        }

        private static ShUser ? mDefaultUser;

        public static ShUser Default =>
            mDefaultUser ??= 
                new ShUser( EntityIdentifier.Default, "Unspecified", "Unspecified", "Unspecified", new List<string>());
    }
}
