using System;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using SpaceHelmet.Client.Auth.Store;
using SpaceHelmet.Client.Auth.Support;
using SpaceHelmet.Client.Constants;
using SpaceHelmet.Client.Shared;
using SpaceHelmet.Client.Support;

namespace SpaceHelmet.Client {
    public interface IAppStartup : IDisposable {
        Task    OnStartup();
        Task    OnLogin();
        void    OnLogout();
    }

    public class AppStartup : IAppStartup {
        private readonly AuthFacade                 mAuthFacade;
        private readonly IAuthInformation           mAuthInformation;
        private readonly NavigationManager          mNavigationManager;
        private readonly ILocalStorageService       mLocalStorage;
        private readonly ITokenExpirationChecker    mTokenChecker;

        public AppStartup( AuthFacade authFacade, NavigationManager navigationManager,
                           ILocalStorageService localStorageService,
                           ITokenExpirationChecker tokenChecker, IAuthInformation authInformation ) {
            mAuthFacade = authFacade;
            mNavigationManager = navigationManager;
            mLocalStorage = localStorageService;
            mTokenChecker = tokenChecker;
            mAuthInformation = authInformation;
        }

        public async Task OnStartup() {
            var token = await mLocalStorage.GetItemAsStringAsync( LocalStorageNames.AuthToken );
            var refresh = await mLocalStorage.GetItemAsStringAsync( LocalStorageNames.RefreshToken );

            if( mAuthInformation.IsTokenValid( token )) {
                mAuthFacade.SetAuthenticationToken( token, refresh );
            }
            else {
                mAuthFacade.ClearAuthenticationToken();

                mNavigationManager.NavigateTo( NavLinks.Login );
            }
        }

        public Task OnLogin() {
            mNavigationManager.NavigateTo( NavLinks.Users );

            mTokenChecker.StartChecking();

            return Task.CompletedTask;
        }

        public void OnLogout() {
            mNavigationManager.NavigateTo( NavLinks.Home );

            mTokenChecker.StopChecking();
        }

        public void Dispose() {
            mTokenChecker.Dispose();
        }
    }
}
