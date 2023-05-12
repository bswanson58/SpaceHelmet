using System.Threading.Tasks;
using Fluxor;
using MudBlazor;
using SpaceHelmet.Client.Ui.Actions;

namespace SpaceHelmet.Client.Ui.Store {
    public class UiFacade {
        private readonly IDialogService mDialogService;
        private readonly IDispatcher mDispatcher;

        public UiFacade( IDialogService dialogService, IDispatcher dispatcher ) {
            mDialogService = dialogService;
            mDispatcher = dispatcher;
        }

        public void ApiCallStarted( string message ) {
            mDispatcher.Dispatch( new ApiCallStarted( message ));
        }

        public void ApiCallCompleted() {
            mDispatcher.Dispatch( new ApiCallCompleted());
        }

        public void ApiCallFailure( string message ) {
            mDispatcher.Dispatch( new ApiCallFailure( message ) );
        }

        public async Task<DialogResult> ConfirmAction( string title, string request ) {
            var parameters = new DialogParameters { { nameof( ConfirmationDialog.Request), request } };
            var options = new DialogOptions { FullWidth = true, CloseOnEscapeKey = true };
            var dialog = await mDialogService.ShowAsync<ConfirmationDialog>( title, parameters, options );

            return await dialog.Result;
        }

        public async Task<DialogResult> DisplayMessage( string title, string message ) {
            var parameters = new DialogParameters { { nameof( MessageDialog.Message ), message } };
            var options = new DialogOptions { FullWidth = true, CloseOnEscapeKey = true };
            var dialog = await mDialogService.ShowAsync<MessageDialog>( title, parameters, options );

            return await dialog.Result;
        }
    }
}
