using System.Threading.Tasks;
using Fluxor;
using SpaceHelmet.Client.Auth.Actions;
using SpaceHelmet.Client.Ui.Store;

namespace SpaceHelmet.Client.Auth.Effects {
    // ReSharper disable once UnusedType.Global
    public class ChangeUserRolesSuccessEffect : Effect<ChangeUserRolesSuccess> {
        private readonly UiFacade   mUiFacade;

        public ChangeUserRolesSuccessEffect( UiFacade uiFacade ) {
            mUiFacade = uiFacade;
        }

        public override async Task HandleAsync( ChangeUserRolesSuccess action, IDispatcher dispatcher ) {
            await mUiFacade.DisplayMessage( "User Roles Change", "Your roles were successfully updated." );
        }
    }
}
