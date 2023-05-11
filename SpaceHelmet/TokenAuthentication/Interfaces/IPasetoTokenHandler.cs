using System.Security.Claims;
using System.Threading.Tasks;
using Paseto.Cryptography.Key;
using TokenAuthentication.Models;
using TokenAuthentication.Settings;

namespace TokenAuthentication.Interfaces {
    public interface IPasetoTokenHandler {
        Task<WebToken>                  WriteTokenAsync( PasetoTokenDescriptor tokenDescriptor, string footer = "" );
        Task<ClaimsPrincipal>           DecodeTokenAsync( string token );

        Task<PasetoAsymmetricKeyPair>   GenerateKeyPairAsync( string secretKey );
    }
}