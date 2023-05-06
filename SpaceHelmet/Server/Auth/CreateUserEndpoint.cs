using Ardalis.ApiEndpoints;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using SpaceHelmet.Shared.Dto.Auth;

namespace SpaceHelmet.Server.Auth {
    [Route(CreateUserRequest.Route)]
    public class CreateUser : EndpointBaseAsync
        .WithRequest<CreateUserRequest>
        .WithActionResult<CreateUserResponse> {

        private readonly IUserService                   mUserService;
        private readonly IValidator<CreateUserRequest>  mValidator;

        public CreateUser( IValidator<CreateUserRequest> validator, IUserService userService ) {
            mValidator = validator;
            mUserService = userService;
        }

        [HttpPost]
        public override async Task<ActionResult<CreateUserResponse>> HandleAsync(
            [FromBody] CreateUserRequest request, 
            CancellationToken cancellationToken = new()) {
            try {
                var validation = await mValidator.ValidateAsync( request, cancellationToken );

                if(!validation.IsValid ) {
                    return Ok( new CreateUserResponse( validation ));
                }

                var user = await mUserService.CreateUser( request.Email, request.Name, request.Password );

                if( user != null ) {
                    return Ok( new CreateUserResponse());
                }

                return Ok( new CreateUserResponse( false, "User could not be created" ));
            }
            catch( Exception ex ) {
                return Ok( new CreateUserResponse( ex ));
            }
        }
    }
}
