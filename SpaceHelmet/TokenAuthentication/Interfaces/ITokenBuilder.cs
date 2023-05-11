using System.Security.Claims;
using System.Threading.Tasks;
using TokenAuthentication.Models;

namespace TokenAuthentication.Interfaces {
    public interface ITokenBuilder {
        Task<WebToken>      GenerateToken( TokenUser user );
        ClaimsPrincipal     GetPrincipalFromExpiredToken( string token );
    }
}
