using Ardalis.ApiEndpoints;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpaceHelmet.Server.Database.Providers;
using SpaceHelmet.Server.Support;
using SpaceHelmet.Shared.Constants;
using SpaceHelmet.Shared.Dto.Users;
using SpaceHelmet.Shared.Entities;

namespace SpaceHelmet.Server.Users {
    [Authorize( Roles = ClaimValues.cAdministrator )]
    [Route( GetUsersRequest.Route )]
    public class GetUsersEndpoint : EndpointBaseAsync 
        .WithRequest<GetUsersRequest>
        .WithActionResult<GetUsersResponse> {

        private readonly IUserProvider                  mUserProvider;
        private readonly IValidator<GetUsersRequest>    mValidator;

        public GetUsersEndpoint( IUserProvider userProvider, IValidator<GetUsersRequest> validator ) {
            mUserProvider = userProvider;
            mValidator = validator;
        }

        [HttpPost]
        public override async Task<ActionResult<GetUsersResponse>> HandleAsync( 
            [FromBody] GetUsersRequest request,
            CancellationToken token = new () ) {
            try {
                var validation = await mValidator.ValidateAsync( request, token );

                if(!validation.IsValid ) {
                    return new ActionResult<GetUsersResponse>( new GetUsersResponse( validation ));
                }

                var userList = PagedList<ShUser>.CreatePagedList( await mUserProvider.GetAll(), request.PageRequest );

                return Ok( new GetUsersResponse( userList, userList.PageInformation ));
            }
            catch( Exception ex ) {
                return Ok( new GetUsersResponse( ex ));
            }
        }
    }
}
