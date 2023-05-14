using System;
using System.Text.Json.Serialization;

namespace TokenClientSupport.Dto {
    public class RefreshTokenRequest {
        public string Token { get; set; }
        public string RefreshToken { get; set; }

        public const string Route = "refresh";

        public RefreshTokenRequest() {
            Token = String.Empty;
            RefreshToken = String.Empty;
        }
    }

    public class RefreshTokenResponse {
        public  bool        Succeeded { get; }
        public  string      Message { get; }
        public  string      Token { get; set; }
        public  string      RefreshToken { get; set; }
        public  DateTime    TokenExpiration { get; set; }

        [JsonConstructor]
        public RefreshTokenResponse( bool succeeded, string message, string token, string refreshToken, DateTime tokenExpiration ) {
            Succeeded = succeeded;
            Message = message;
            Token = token;
            RefreshToken = refreshToken;
            TokenExpiration = tokenExpiration;
        }

        public RefreshTokenResponse( string token, string refreshToken, DateTime tokenExpiration ) {
            Succeeded = true;
            Message = String.Empty;
            Token = token;
            RefreshToken = refreshToken;
            TokenExpiration = tokenExpiration;
        }

        public RefreshTokenResponse( string error ) {
            Succeeded = false;
            Message = error;
            Token = String.Empty;
            RefreshToken = String.Empty;
            TokenExpiration = DateTime.MinValue;
        }
    }
}
