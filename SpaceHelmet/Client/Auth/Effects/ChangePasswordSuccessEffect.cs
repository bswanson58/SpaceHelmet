using System.Threading.Tasks;
using Fluxor;
using SpaceHelmet.Client.Auth.Actions;
using SpaceHelmet.Client.Ui.Store;

namespace SpaceHelmet.Client.Auth.Effects {
    // ReSharper disable once UnusedType.Global
    public class ChangePasswordSuccessEffect : Effect<ChangePasswordSuccess> {
        private readonly UiFacade   mUiFacade;

        public ChangePasswordSuccessEffect( UiFacade uiFacade ) {
            mUiFacade = uiFacade;
        }

        public override async Task HandleAsync( ChangePasswordSuccess action, IDispatcher dispatcher ) {
            await mUiFacade.DisplayMessage( "Password Change", "Your password was successfully updated." );
        }
    }
}
