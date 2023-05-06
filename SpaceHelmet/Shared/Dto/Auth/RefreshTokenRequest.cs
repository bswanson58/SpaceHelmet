using System.Text.Json.Serialization;
using SpaceHelmet.Shared.Constants;

namespace SpaceHelmet.Shared.Dto.Auth {
    public class RefreshTokenRequest {
        public string Token { get; set; }
        public string RefreshToken { get; set; }

        public const string Route = $"{Routes.BaseRoute}/refresh";

        public RefreshTokenRequest() {
            Token = String.Empty;
            RefreshToken = String.Empty;
        }
    }

    public class RefreshTokenResponse : BaseResponse {
        public string Token { get; set; }
        public string RefreshToken { get; set; }

        [JsonConstructor]
        public RefreshTokenResponse( bool succeeded, string message, string token, string refreshToken ) :
            base( succeeded, message ) {
            Token = token;
            RefreshToken = refreshToken;
        }

        public RefreshTokenResponse( string token, string refreshToken ) {
            Token = token;
            RefreshToken = refreshToken;
        }

        public RefreshTokenResponse( string error ) :
            base( false, error ) {
            Token = String.Empty;
            RefreshToken = String.Empty;
        }
    }
}
