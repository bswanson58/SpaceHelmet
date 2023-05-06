using System.Threading.Tasks;
using Fluxor;
using SpaceHelmet.Client.Auth.Actions;
using SpaceHelmet.Client.Ui.Store;

namespace SpaceHelmet.Client.Auth.Effects {
    // ReSharper disable once UnusedType.Global
    public class CreateUserSuccessEffect : Effect<CreateUserSuccessAction> {
        private readonly UiFacade   mUiFacade;
        public CreateUserSuccessEffect( UiFacade uiFacade ) {
            mUiFacade = uiFacade;
        }

        public override async Task HandleAsync( CreateUserSuccessAction action, IDispatcher dispatcher ) {
            await mUiFacade.DisplayMessage( "Create User", "The user was successfully created!" );
        }
    }
}
