namespace TokenClientSupport.Constants {
    public static class ClaimValues {
        public static readonly string   ClaimEmail      = "email";
        public static readonly string   ClaimEmailHash  = "emailHash";
        public static readonly string   ClaimEntityId   = "entityId";
        public static readonly string   ClaimName       = "name";
        public static readonly string   Expiration      = "exp";
        public static readonly string   RefreshName     = "refreshName";

        public static readonly string   ClaimRoleAdmin  = cAdministrator;
        public static readonly string   ClaimRoleUser   = cUser;

        public const string             cAdministrator  = "administrator";
        public const string             cUser           = "user";
    }
}
