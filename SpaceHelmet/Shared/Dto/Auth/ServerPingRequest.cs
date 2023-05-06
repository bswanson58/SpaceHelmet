using System.Text.Json.Serialization;
using SpaceHelmet.Shared.Constants;

namespace SpaceHelmet.Shared.Dto.Auth {
    public class ServerPingRequest {

        public const string Route = $"{Routes.BaseRoute}/serverPing";

        [JsonConstructor]
        public ServerPingRequest() { }
    }

    public class ServerPingResponse : BaseResponse {
        [JsonConstructor]
        public ServerPingResponse( bool succeeded, string message ) :
            base( succeeded, message ) {
        }

        public ServerPingResponse() { }

        public ServerPingResponse( Exception ex ) :
            base( ex ) {
        }
    }
}
