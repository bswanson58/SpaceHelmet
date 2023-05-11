using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using TokenAuthentication.Models;

namespace TokenAuthentication.Interfaces {
    public interface IClaimBuilder {
        Task<IEnumerable<Claim>>    GetClaimsAsync( TokenUser user );
        Task<IEnumerable<string>>   GetRolesAsync( TokenUser user );
    }
}
