using System.Text.Json.Serialization;
using FluentValidation;
using FluentValidation.Results;
using SpaceHelmet.Shared.Constants;

namespace SpaceHelmet.Shared.Dto.ClientApi {
    public class ClientRequest {
        public  string      SomeData { get; }

        public const string Route = $"{Routes.BaseRoute}/clientData";

        [JsonConstructor]
        public ClientRequest( string someData ) {
            SomeData = someData;
        }
    }

    public class ClientResponse : BaseResponse {
        public  string      ClientData { get; }

        [JsonConstructor]
        public ClientResponse( bool succeeded, string message, string clientData ) :
            base( succeeded, message ) {
            ClientData = clientData;
        }

        public ClientResponse( string clientData ) {
            ClientData = clientData;
        }

        public ClientResponse( ValidationResult validationResult ) :
            base ( validationResult ) {
            ClientData = String.Empty;
        }

        public ClientResponse( Exception ex ) :
            base( ex ) {
            ClientData = String.Empty;
        }
    }

    public class ClientRequestValidator : AbstractValidator<ClientRequest> {
        public ClientRequestValidator() {
            RuleFor( c => c.SomeData )
                .NotEmpty()
                .WithMessage( "ClientData cannot be empty" );
        }
    }
}
