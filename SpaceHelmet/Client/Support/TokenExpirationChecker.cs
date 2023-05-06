using System;
using System.Threading;
using SpaceHelmet.Client.Auth.Store;
using SpaceHelmet.Client.Auth.Support;

namespace SpaceHelmet.Client.Support {
    public interface ITokenExpirationChecker : IDisposable {
        void    StartChecking();
        void    StopChecking();
    }

    public class TokenExpirationChecker : ITokenExpirationChecker {
        private const int                   cCheckTimeInMinutes = 1;

        private readonly IAuthInformation   mAuthInformation;
        private readonly AuthFacade         mAuthFacade;
        private Timer ?                     mTimer;
        private bool                        mDidIt;

        public TokenExpirationChecker( AuthFacade authFacade, IAuthInformation authInformation ) {
            mAuthFacade = authFacade;
            mAuthInformation = authInformation;

            mTimer = null;
            mDidIt = false;
        }

        public void StartChecking() {
            StopChecking();

            mDidIt = false;

            mTimer = new Timer( OnTimer, null, 
                TimeSpan.FromMinutes( cCheckTimeInMinutes ), 
                TimeSpan.FromMinutes( cCheckTimeInMinutes ));
        }

        private void OnTimer( object ? state ) {
            if( TokenRefreshImmanent( TimeSpan.FromMinutes( cCheckTimeInMinutes ))) {
                if(!mDidIt ) {
                    mAuthFacade.LogoutUser();

                    mDidIt = true;
                }
            }
            else {
                mDidIt = false;
            }
        }

        private bool TokenRefreshImmanent( TimeSpan timeOffset ) =>
            mAuthInformation.TimeOffsetToTokenExpiration < timeOffset;

        public void StopChecking() {
            mTimer?.Dispose();
            mTimer = null;
        }

        public void Dispose() {
            StopChecking();
        }
    }
}
