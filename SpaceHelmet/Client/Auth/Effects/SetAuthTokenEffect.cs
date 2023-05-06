using System;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Fluxor;
using Microsoft.AspNetCore.Components.Authorization;
using SpaceHelmet.Client.Auth.Actions;
using SpaceHelmet.Client.Auth.Store;
using SpaceHelmet.Client.Auth.Support;
using SpaceHelmet.Client.Constants;

namespace SpaceHelmet.Client.Auth.Effects {
    // ReSharper disable once UnusedType.Global
    public class SetAuthTokenEffect : Effect<SetAuthToken> {
        private readonly IAppStartup                    mAppStartup;
        private readonly AuthenticationStateProvider    mAuthStateProvider;
        private readonly AuthFacade                     mAuthFacade;
        private readonly ILocalStorageService           mLocalStorage;

        public SetAuthTokenEffect( AuthenticationStateProvider authStateProvider, AuthFacade authFacade,
                                   ILocalStorageService localStorage, IAppStartup appStartup ) {
            mAuthStateProvider = authStateProvider;
            mAuthFacade = authFacade;
            mLocalStorage = localStorage;
            mAppStartup = appStartup;
        }

        public override async Task HandleAsync( SetAuthToken action, IDispatcher dispatcher ) {
            if( String.IsNullOrWhiteSpace( action.Token )) {
                await mLocalStorage.RemoveItemAsync( LocalStorageNames.AuthToken );
                await mLocalStorage.RemoveItemAsync( LocalStorageNames.RefreshToken );
            }
            else {
                await mLocalStorage.SetItemAsStringAsync( LocalStorageNames.AuthToken, action.Token );
                await mLocalStorage.SetItemAsStringAsync( LocalStorageNames.RefreshToken, action.RefreshToken );
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
