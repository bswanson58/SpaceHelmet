using System.Security.Claims;
using System.Threading.Tasks;

namespace PasetoAuth.Interfaces {
    public interface IPasetoRefreshTokenProvider {
        Task<string>            CreateAsync( ClaimsIdentity claimsPrincipal );
    }
}