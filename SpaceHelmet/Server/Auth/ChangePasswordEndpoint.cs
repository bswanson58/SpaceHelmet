using Ardalis.ApiEndpoints;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpaceHelmet.Server.Database.Providers;
using SpaceHelmet.Shared.Dto.Auth;

namespace SpaceHelmet.Server.Auth {
    [Authorize]
    [Route( ChangePasswordRequest.Route )]
    public class ChangePasswordEndpoint : EndpointBaseAsync
        .WithRequest<ChangePasswordRequest>
        .WithActionResult<ChangePasswordResponse> {

        private readonly IUserService                       mUserService;
        private readonly IUserProvider                      mUserProvider;
        private readonly IValidator<ChangePasswordRequest>  mValidator;

        public ChangePasswordEndpoint( IUserService userService, IUserProvider userProvider,
            IValidator<ChangePasswordRequest> validator ) {
            mUserService = userService;
            mUserProvider = userProvider;
            mValidator = validator;
        }

        [HttpPost]
        public override async Task<ActionResult<ChangePasswordResponse>> HandleAsync( 
            [FromBody] ChangePasswordRequest request,
            CancellationToken cancellationToken = new () ) {
            try {
                var validation = await mValidator.ValidateAsync( request, cancellationToken );

                if(!validation.IsValid ) {
                    return Ok( new ChangePasswordResponse( validation ));
                }

                var user = await mUserProvider.GetFromContext( HttpContext );

                if( user == null ) {
                    return Ok( new ChangePasswordResponse( false, "User could not be located" ));
                }

                await mUserService.UpdatePassword( user, request.CurrentPassword, request.Password );

                return Ok( new ChangePasswordResponse());

            }
            catch( Exception ex ) {
                return Ok( new ChangePasswordResponse( ex ));
            }
        }
    }
}
