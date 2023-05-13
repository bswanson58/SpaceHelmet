using System;
using System.Threading.Tasks;
using Fluxor;
using Microsoft.AspNetCore.Components.Authorization;
using SpaceHelmet.Client.Auth.Actions;
using SpaceHelmet.Client.Auth.Store;
using SpaceHelmet.Client.Auth.Support;
using TokenClientSupport.Interfaces;

namespace SpaceHelmet.Client.Auth.Effects {
    // ReSharper disable once UnusedType.Global
    public class SetAuthTokenEffect : Effect<SetAuthToken> {
        private readonly IAppStartup                    mAppStartup;
        private readonly AuthenticationStateProvider    mAuthStateProvider;
        private readonly AuthFacade                     mAuthFacade;
        private readonly ITokenStorageProvider          mTokenProvider;

        public SetAuthTokenEffect( AuthenticationStateProvider authStateProvider, AuthFacade authFacade,
                                   ITokenStorageProvider tokenProvider, IAppStartup appStartup ) {
            mAuthStateProvider = authStateProvider;
            mAuthFacade = authFacade;
            mTokenProvider = tokenProvider;
            mAppStartup = appStartup;
        }

        public override async Task HandleAsync( SetAuthToken action, IDispatcher dispatcher ) {
            if( String.IsNullOrWhiteSpace( action.Token )) {
                await mTokenProvider.ClearTokenValues();
            }
            else {
                await mTokenProvider.StoreAuthenticationToken( action.Token );
                await mTokenProvider.StoreRefreshToken( action.RefreshToken );
            }

            if( mAuthStateProvider is AuthStateProvider authProvider ) {
                authProvider.SetUserAuthentication();
            }

            mAuthFacade.AnnounceAuthStateUpdated( action.IsRefresh );

            if((!String.IsNullOrWhiteSpace( action.Token )) &&
               (!action.IsRefresh )) {
                await mAppStartup.OnLogin();
            }
        }
    }
}
