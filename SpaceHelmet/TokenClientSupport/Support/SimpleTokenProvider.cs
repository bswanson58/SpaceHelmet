using System;
using System.Threading;
using System.Threading.Tasks;
using TokenClientSupport.Interfaces;

namespace TokenClientSupport.Support {
    internal class SimpleTokenProvider : ITokenStorageProvider {
        private string  mAuthenticationToken;
        private string  mRefreshToken;

        public SimpleTokenProvider() {
            mAuthenticationToken = String.Empty;
            mRefreshToken = String.Empty;
        }

        public Task ClearTokenValues() {
            mAuthenticationToken = String.Empty;
            mRefreshToken = String.Empty;

            return Task.CompletedTask;
        }

        public Task<string> GetAuthenticationToken( CancellationToken token = new CancellationToken()) =>
            Task.FromResult( mAuthenticationToken );

        public Task StoreAuthenticationToken( string token ) {
            mAuthenticationToken = token;

            return Task.CompletedTask;
        }

        public Task<string> GetRefreshToken( CancellationToken token = new CancellationToken()) =>
            Task.FromResult( mRefreshToken );

        public Task StoreRefreshToken( string token ) {
            mRefreshToken = token;

            return Task.CompletedTask;
        }
    }
}
