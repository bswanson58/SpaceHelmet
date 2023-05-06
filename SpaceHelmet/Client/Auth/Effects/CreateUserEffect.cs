using System.Threading.Tasks;
using SpaceHelmet.Shared.Dto.Auth;
using Fluxor;
using MudBlazor;
using SpaceHelmet.Client.Auth.Actions;

namespace SpaceHelmet.Client.Auth.Effects {
    // ReSharper disable once UnusedType.Global
    public class CreateUserEffect : Effect<CreateUserAction> {
        private readonly IDialogService mDialogService;

        public CreateUserEffect( IDialogService dialogService ) {
            mDialogService = dialogService;
        }

        public override async Task HandleAsync( CreateUserAction action, IDispatcher dispatcher ) {
            var parameters = new DialogParameters { { "user", new CreateUserRequest() } };
            var options = new DialogOptions { FullWidth = true, CloseOnEscapeKey = true };
            var dialog = await mDialogService.ShowAsync<CreateUserDialog>( "Register User", parameters, options );
            var dialogResult = await dialog.Result;
        
            if((!dialogResult.Canceled ) &&
               ( dialogResult.Data is CreateUserRequest userInput )) {
                dispatcher.Dispatch( new CreateUserSubmitAction( userInput ));
            }
        }
    }
}
