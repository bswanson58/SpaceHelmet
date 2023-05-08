using System.Security.Claims;
using System.Threading.Tasks;
using Paseto.Cryptography.Key;
using PasetoAuth.Common;

namespace PasetoAuth.Interfaces {
    public interface IPasetoTokenHandler {
        Task<PasetoToken>               WriteTokenAsync( PasetoTokenDescriptor tokenDescriptor, string footer = "" );
        Task<ClaimsPrincipal>           DecodeTokenAsync( string token );

        Task<PasetoAsymmetricKeyPair>   GenerateKeyPairAsync( string secretKey );
    }
}