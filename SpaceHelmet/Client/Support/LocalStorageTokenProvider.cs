using System.Threading;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using TokenClientSupport.Constants;
using TokenClientSupport.Interfaces;

namespace SpaceHelmet.Client.Support {
    internal class LocalStorageTokenProvider : ITokenStorageProvider {
        private readonly ILocalStorageService   mLocalStorage;

        public LocalStorageTokenProvider( ILocalStorageService localStorageService ) {
            mLocalStorage = localStorageService;
        }

        public async Task ClearTokenValues() {
            await mLocalStorage.RemoveItemAsync( TokenStorageNames.AuthToken );
            await mLocalStorage.RemoveItemAsync( TokenStorageNames.RefreshToken );
        }

        public async Task<string> GetAuthenticationToken( CancellationToken token = new ()) =>
            await mLocalStorage.GetItemAsStringAsync( TokenStorageNames.AuthToken, token );

        public async Task StoreAuthenticationToken( string token ) {
            await mLocalStorage.SetItemAsStringAsync( TokenStorageNames.AuthToken, token );
        }

        public async Task<string> GetRefreshToken( CancellationToken token = new ()) =>
            await mLocalStorage.GetItemAsStringAsync( TokenStorageNames.RefreshToken, token );

        public async Task StoreRefreshToken( string token ) {
            await mLocalStorage.SetItemAsStringAsync( TokenStorageNames.RefreshToken, token );
        }
    }
}
