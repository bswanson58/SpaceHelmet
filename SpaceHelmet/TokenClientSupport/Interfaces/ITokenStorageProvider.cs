using System.Threading;
using System.Threading.Tasks;

namespace TokenClientSupport.Interfaces {
    public interface ITokenStorageProvider {
        Task            ClearTokenValues();

        Task<string>    GetAuthenticationToken( CancellationToken token = new());
        Task            StoreAuthenticationToken( string token );

        Task<string>    GetRefreshToken( CancellationToken token = new());
        Task            StoreRefreshToken( string token );
    }
}
