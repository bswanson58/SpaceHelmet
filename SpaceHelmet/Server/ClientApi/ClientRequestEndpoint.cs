using Ardalis.ApiEndpoints;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpaceHelmet.Shared.Constants;
using SpaceHelmet.Shared.Dto.ClientApi;

namespace SpaceHelmet.Server.ClientApi {
    [Route( ClientRequest.Route)]
    [Authorize( Roles = ClaimValues.cAdministrator )]
    public class ClientRequestEndpoint : EndpointBaseAsync
        .WithRequest<ClientRequest>
        .WithActionResult<ClientResponse> {
        private readonly IValidator<ClientRequest>  mValidator;

        public ClientRequestEndpoint( IValidator<ClientRequest> validator ) {
            mValidator = validator;
        }

        public override async Task<ActionResult<ClientResponse>> HandleAsync( 
            ClientRequest request, 
            CancellationToken cancellationToken = new ()) {
            try {
                var validation = await mValidator.ValidateAsync( request, cancellationToken );

                if(!validation.IsValid ) {
                    return new ActionResult<ClientResponse>( new ClientResponse( validation ));
                }

                return Ok( new ClientResponse( "some client data" ));

            }
            catch( Exception ex ) {
                return BadRequest( new ClientResponse( ex ));
            }
        }
    }
}
