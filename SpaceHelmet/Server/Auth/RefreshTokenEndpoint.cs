using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SpaceHelmet.Server.Database.Entities;
using SpaceHelmet.Shared.Dto.Auth;
using SpaceHelmet.Shared.Support;

namespace SpaceHelmet.Server.Auth {
    [Route( RefreshTokenRequest.Route )]
    public class RefreshToken : EndpointBaseAsync
        .WithRequest<RefreshTokenRequest>
        .WithActionResult<RefreshTokenResponse> {

        private readonly UserManager<DbUser>    mUserManager;
        private readonly ITokenBuilder          mTokenBuilder;

        public RefreshToken( UserManager<DbUser> userManager, ITokenBuilder tokenBuilder ) {
            mUserManager = userManager;
            mTokenBuilder = tokenBuilder;
        }

        [HttpPost]
        public override async Task<ActionResult<RefreshTokenResponse>> HandleAsync( 
            [FromBody] RefreshTokenRequest request,
            CancellationToken cancellationToken = new () ) {

            var principal = mTokenBuilder.GetPrincipalFromExpiredToken( request.Token );
            var user = default( DbUser );

            if(!String.IsNullOrWhiteSpace( principal.Identity?.Name )) {
                user = await mUserManager.FindByEmailAsync( principal.Identity.Name );
            }

            if(( user == null ) ||
               ( user.RefreshToken != request.RefreshToken ) ||
               ( user.RefreshTokenExpiration <= DateTimeProvider.Instance.CurrentDateTime )) {
                return Unauthorized( new RefreshTokenResponse( "Invalid client request" ));
            }

            var token = await mTokenBuilder.GenerateToken( user );

//            user.RefreshToken = mTokenBuilder.GenerateRefreshToken();
//            user.RefreshTokenExpiration = mTokenBuilder.TokenExpiration();

//            await mUserManager.UpdateAsync( user );

            return Ok( new RefreshTokenResponse( token.Token, user.RefreshToken ));
        }
    }
}
