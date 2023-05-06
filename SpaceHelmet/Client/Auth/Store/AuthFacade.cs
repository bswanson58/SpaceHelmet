using System;
using SpaceHelmet.Shared.Dto.Auth;
using Fluxor;
using Microsoft.AspNetCore.Components;
using SpaceHelmet.Client.Auth.Actions;
using SpaceHelmet.Client.Shared;
using SpaceHelmet.Shared.Entities;

namespace SpaceHelmet.Client.Auth.Store {
    public class AuthFacade {
        private readonly IDispatcher        mDispatcher;
        private readonly NavigationManager  mNavigationManager;

        public AuthFacade( IDispatcher dispatcher, NavigationManager navigationManager ) {
            mDispatcher = dispatcher;
            mNavigationManager = navigationManager;
        }

        public void SetAuthenticationToken( string token, string refreshToken ) {
            mDispatcher.Dispatch( new SetAuthToken( token, refreshToken, false ));
        }

        public void ClearAuthenticationToken() {
            mDispatcher.Dispatch( new SetAuthToken( String.Empty, String.Empty, false ));
        }

        public void SetRefreshToken( string token, string refreshToken ) {
            mDispatcher.Dispatch( new SetAuthToken( token, refreshToken, true ));
        }

        public void AnnounceAuthStateUpdated( bool wasFresh ) {
            mDispatcher.Dispatch( new AuthStateUpdated( wasFresh ));
        }

        public void RegisterUser() {
            mDispatcher.Dispatch( new CreateUserAction());
        }

        public void LoginUser() {
            mNavigationManager.NavigateTo( NavLinks.Login );
        }

        public void LoginUser( LoginUserRequest input ) {
            mDispatcher.Dispatch( new LoginUserSubmitAction( input ));
        }

        public void ChangePassword() {
            mDispatcher.Dispatch( new ChangePasswordAction());
        }

        public void ChangeUserRoles( ShUser user ) {
            mDispatcher.Dispatch( new ChangeUserRolesAction( user ));
        }
        public void LogoutUser() {
            mDispatcher.Dispatch( new LogoutUserAction());
        }
    }
}
