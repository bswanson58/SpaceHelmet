namespace TokenClientSupport.Settings {
    public class ClientTokenOptions {
        public  bool        UseTokens { get; set; }
        public  bool        UseRefreshTokens { get; set; }
        public  string      TokenStyle { get; set; }

        public ClientTokenOptions() {
            UseTokens = false;
            UseRefreshTokens = false;
            TokenStyle = TokenStyles.JsonWebTokens;
        }
    }
}
