using System;
using System.Net.Http;
using System.Threading.Tasks;
using SpaceHelmet.Shared.Dto.Auth;
using Fluxor;
using Microsoft.Extensions.Logging;
using SpaceHelmet.Client.Auth.Actions;
using SpaceHelmet.Client.Support;
using SpaceHelmet.Client.Ui.Actions;

namespace SpaceHelmet.Client.Auth.Effects {
    // ReSharper disable once UnusedType.Global
    public class ChangePasswordSubmitEffect : Effect<ChangePasswordSubmit> {
        private readonly ILogger<ChangePasswordSubmitEffect>    mLogger;
        private readonly IAuthenticatedHttpHandler              mHttpHandler;

        public ChangePasswordSubmitEffect( ILogger<ChangePasswordSubmitEffect> logger, IAuthenticatedHttpHandler httpHandler ) {
            mLogger = logger;
            mHttpHandler = httpHandler;
        }

        public override async Task HandleAsync( ChangePasswordSubmit action, IDispatcher dispatcher ) {
            dispatcher.Dispatch( new ApiCallStarted( "Updating Password" ));

            try {
                var response = await mHttpHandler.Post<ChangePasswordResponse>( ChangePasswordRequest.Route, action.Request );

                if( response != null ) {
                    if( response.Succeeded ) {
                        dispatcher.Dispatch( new ChangePasswordSuccess());
                    }
                    else {
                        dispatcher.Dispatch( new ChangePasswordFailure( response.Message ));
                    }
                }
                else {
                    dispatcher.Dispatch( new ChangePasswordFailure( "Received null response" ));
                }
            }
            catch ( HttpRequestException exception ) {
                mLogger.LogError( exception, String.Empty );

                dispatcher.Dispatch( new ChangePasswordFailure( exception.Message ));
            }

            dispatcher.Dispatch( new ApiCallCompleted());
        }
    }
}
