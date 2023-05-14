using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SpaceHelmet.Server.Database.Entities;
using SpaceHelmet.Shared.Constants;
using SpaceHelmet.Shared.Support;
using TokenAuthentication.Interfaces;
using TokenClientSupport.Dto;

namespace SpaceHelmet.Server.Auth {
    [Route( $"{Routes.BaseRoute}/{RefreshTokenRequest.Route}" )]
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

            var principal = await mTokenBuilder.GetPrincipalFromExpiredToken( request.Token );
            var refreshName = principal.Claims.FirstOrDefault( c => c.Type.Equals( ClaimValues.RefreshName ));
            var user = default( DbUser );

            if(!String.IsNullOrWhiteSpace( refreshName?.Value )) {
                user = await mUserManager.FindByEmailAsync( refreshName.Value );
            }

            if(( user == null ) ||
               ( user.RefreshToken != request.RefreshToken ) ||
               ( user.RefreshTokenExpiration <= DateTimeProvider.Instance.CurrentUtcTime )) {
                return Unauthorized( new RefreshTokenResponse( "Invalid client request" ));
            }

            var token = await mTokenBuilder.GenerateToken( user );

            user.RefreshToken = token.RefreshToken;
            user.RefreshTokenExpiration = token.RefreshExpiresAt;

            await mUserManager.UpdateAsync( user );

            var expirationTime = token.RefreshExpiresAt > token.ExpiresAt ? 
                token.RefreshExpiresAt : 
                token.ExpiresAt;

            return Ok( new RefreshTokenResponse( token.Token, token.RefreshToken, expirationTime ));
        }
    }
}
