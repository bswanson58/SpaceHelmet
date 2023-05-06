using Ardalis.ApiEndpoints;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SpaceHelmet.Server.Database.Entities;
using SpaceHelmet.Shared.Dto.Auth;

namespace SpaceHelmet.Server.Auth {
    [AllowAnonymous]
    [Route( LoginUserRequest.Route )]
    public class LoginUser : EndpointBaseAsync
        .WithRequest<LoginUserRequest>
        .WithActionResult<LoginUserResponse> {

        private readonly UserManager<DbUser>        mUserManager;
        private readonly ITokenBuilder              mTokenBuilder;
        private readonly IValidator<LoginUserRequest> mValidator;

        public LoginUser( UserManager<DbUser> userManager, ITokenBuilder tokenBuilder, IValidator<LoginUserRequest> validator ) {
            mUserManager = userManager;
            mTokenBuilder = tokenBuilder;
            mValidator = validator;
        }

        [HttpPost]
        public override async Task<ActionResult<LoginUserResponse>> HandleAsync( 
            [FromBody] LoginUserRequest request, 
            CancellationToken cancellationToken = new()) {
            try {
                var validation = await mValidator.ValidateAsync( request, cancellationToken );

                if(!validation.IsValid ) {
                    return Ok( new LoginUserResponse( validation ));
                }

                var user = await mUserManager.FindByNameAsync( request.LoginName );

                if(( user == null ) ||
                   (!await mUserManager.CheckPasswordAsync( user, request.Password ))) {
                    return Unauthorized( new LoginUserResponse( "Invalid Authentication" ));
                }

                var token = await mTokenBuilder.GenerateToken( user );

                user.RefreshToken = mTokenBuilder.GenerateRefreshToken();
                user.RefreshTokenExpiration = mTokenBuilder.TokenExpiration();

                await mUserManager.UpdateAsync( user );

                return new LoginUserResponse( token, user.RefreshToken, user.RefreshTokenExpiration );
            }
            catch( Exception ex ) {
                return Ok( new LoginUserResponse( ex ));
            }
        }
    }
}
