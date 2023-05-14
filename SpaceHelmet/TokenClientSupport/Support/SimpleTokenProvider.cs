using System;
using System.Threading;
using System.Threading.Tasks;
using TokenClientSupport.Interfaces;

namespace TokenClientSupport.Support {
    internal class SimpleTokenProvider : ITokenStorageProvider {
        private string      mAuthenticationToken;
        private string      mRefreshToken;
        private DateTime    mExpiration;

        public SimpleTokenProvider() {
            mAuthenticationToken = String.Empty;
            mRefreshToken = String.Empty;
            mExpiration = DateTime.MinValue;
        }

        public Task ClearTokenValues() {
            mAuthenticationToken = String.Empty;
            mRefreshToken = String.Empty;
            mExpiration = DateTime.MinValue;

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

        public Task<DateTime> GetTokenExpiration( CancellationToken token = new CancellationToken()) =>
            Task.FromResult( mExpiration );

        public Task StoreTokenExpiration( DateTime expiration ) {
            mExpiration = expiration;

            return Task.CompletedTask;
        }
    }
}
