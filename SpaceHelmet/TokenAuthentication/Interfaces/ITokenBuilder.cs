using System.Security.Claims;
using System.Threading.Tasks;
using TokenAuthentication.Models;

namespace TokenAuthentication.Interfaces {
    public interface ITokenBuilder {
        Task<WebToken>          GenerateToken( TokenUser user );
        Task<ClaimsPrincipal>   GetPrincipalFromExpiredToken( string token );
    }
}
