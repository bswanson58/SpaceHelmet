using System.Threading.Tasks;
using SpaceHelmet.Shared.Dto.Auth;
using Fluxor;
using MudBlazor;
using SpaceHelmet.Client.Auth.Actions;

namespace SpaceHelmet.Client.Auth.Effects {
    // ReSharper disable once UnusedType.Global
    public class ChangeUserRolesEffect : Effect<ChangeUserRolesAction> {
        private readonly IDialogService mDialogService;

        public ChangeUserRolesEffect( IDialogService dialogService ) {
            mDialogService = dialogService;
        }

        public override async Task HandleAsync( ChangeUserRolesAction action, IDispatcher dispatcher ) {
            var inputRequest = new ChangeUserRolesRequest( action.User );
            var parameters = new DialogParameters { { nameof( ChangeUserRolesDialog.Request ), inputRequest } };
            var options = new DialogOptions { FullWidth = true, CloseOnEscapeKey = true };
            var dialog = await mDialogService.ShowAsync<ChangeUserRolesDialog>( "Change User Roles", parameters, options );
            var dialogResult = await dialog.Result;
        
            if((!dialogResult.Canceled ) &&
               ( dialogResult.Data is ChangeUserRolesRequest request )) {
                dispatcher.Dispatch( new ChangeUserRolesSubmit( request ));
            }
        }
    }
}
