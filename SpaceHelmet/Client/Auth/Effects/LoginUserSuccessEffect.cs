using System.Threading.Tasks;
using Fluxor;
using SpaceHelmet.Client.Auth.Actions;
using SpaceHelmet.Client.Auth.Store;

namespace SpaceHelmet.Client.Auth.Effects {
    // ReSharper disable once UnusedType.Global
    public class LoginUserSuccessEffect : Effect<LoginUserSuccessAction> {
        private readonly AuthFacade     mAuthFacade;

        public LoginUserSuccessEffect( AuthFacade authFacade ) {
            mAuthFacade = authFacade;
        }

        public override Task HandleAsync( LoginUserSuccessAction action, IDispatcher dispatcher ) {
            mAuthFacade.SetAuthenticationToken( action.UserResponse.Token, action.UserResponse.RefreshToken, 
                                                action.UserResponse.Expiration );

            return Task.CompletedTask;
        }
    }
}
