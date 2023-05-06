using Microsoft.AspNetCore.Identity;
using SpaceHelmet.Server.Database.Entities;
using SpaceHelmet.Shared.Constants;
using SpaceHelmet.Shared.Entities;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace SpaceHelmet.Server.Database.Providers {
    public interface IUserProvider {
        Task<IList<ShUser>>     GetAll();
        ValueTask<ShUser ?>     GetById( string id );
        ValueTask<ShUser ?>     GetFromContext( HttpContext context );
    }

    public class TcUserProvider : IUserProvider {
        private readonly SpaceHelmetDbContext   mContext;
        private readonly UserManager<DbUser>    mUserManager;

        public TcUserProvider( SpaceHelmetDbContext context, UserManager<DbUser> userManager ) {
            mContext = context;
            mUserManager = userManager;
        }

        public async Task<IList<ShUser>> GetAll() {
            var retValue = new List<ShUser>();
            var users = await mContext.Users.ToListAsync();

            foreach( var user in users ) {
                retValue.Add( await ConvertTo( user ));
            }

            return retValue;
        }

        public async ValueTask<ShUser ?> GetById( string id ) {
            if(( String.IsNullOrWhiteSpace( id )) ||
               ( id.Equals( ShUser.Default.EntityId ))) {
                return null;
            }

            var user = await mContext.Users.FindAsync( id );

            return user != null ? await ConvertTo( user ) : null;
        }

        public ValueTask<ShUser ?> GetFromContext( HttpContext context ) {
            var userId = context.User.Claims
                .FirstOrDefault( c => c.Type.Equals( ClaimValues.ClaimEntityId ))?.Value ?? String.Empty;

            if(!String.IsNullOrWhiteSpace( userId )) {
                return GetById( userId );
            }

            return default;
        }

        private async Task<ShUser> ConvertTo( DbUser user ) {
            var claims = await mUserManager.GetClaimsAsync( user );
            var nameClaim = claims.FirstOrDefault( c => c.Type.Equals( ClaimTypes.GivenName ));
            var name = nameClaim != null ? nameClaim.Value : String.Empty;

            var roles = await mUserManager.GetRolesAsync( user );

            return new ShUser( user.Id, user.UserName ?? String.Empty, name, user.Email ?? String.Empty, roles );
        }
    }
}
