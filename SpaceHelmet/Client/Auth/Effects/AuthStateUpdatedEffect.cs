using System.Threading.Tasks;
using Fluxor;
using SpaceHelmet.Client.Auth.Actions;
using SpaceHelmet.Client.Auth.Support;

namespace SpaceHelmet.Client.Auth.Effects {
    // ReSharper disable once UnusedType.Global
    public class AuthStateUpdatedEffect : Effect<AuthStateUpdated> {
        private readonly IAuthInformation   mAuthInformation;

        public AuthStateUpdatedEffect( IAuthInformation authInformation ) {
            mAuthInformation = authInformation;
        }

        public override Task HandleAsync( AuthStateUpdated action, IDispatcher dispatcher ) {
            if( mAuthInformation.IsAuthValid ) {
//                mUserDataFacade.GetUserData();
            }

            return Task.CompletedTask;
        }
    }
}
