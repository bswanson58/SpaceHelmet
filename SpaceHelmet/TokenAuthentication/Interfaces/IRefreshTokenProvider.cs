using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace TokenAuthentication.Interfaces {
    public interface IRefreshTokenProvider {
        Task<string>    CreateAsync( ClaimsIdentity claimsPrincipal );
        DateTime        TokenExpiration();
    }
}
