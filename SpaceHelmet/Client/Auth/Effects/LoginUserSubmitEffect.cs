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
    public class LoginUserSubmitEffect : Effect<LoginUserSubmitAction> {
        private readonly ILogger<LoginUserSubmitEffect> mLogger;
        private readonly IAnonymousHttpHandler          mHttpHandler;

        public LoginUserSubmitEffect( IAnonymousHttpHandler httpHandler, ILogger<LoginUserSubmitEffect> logger ) {
            mHttpHandler = httpHandler;
            mLogger = logger;
        }

        public override async Task HandleAsync( LoginUserSubmitAction action, IDispatcher dispatcher ) {
            dispatcher.Dispatch( new ApiCallStarted( "Requesting User Authentication" ));

            try {
                var response = await mHttpHandler.Post<LoginUserResponse>( LoginUserRequest.Route, action.UserInput  );

                if( response?.Succeeded == true ) {
                    dispatcher.Dispatch( new LoginUserSuccessAction( response ));
                }
                else {
                    dispatcher.Dispatch( new LoginUserFailureAction( response?.Message ?? "Received null response" ));
                }
            }
            catch( HttpRequestException exception ) {
                mLogger.LogError( exception, String.Empty );

                dispatcher.Dispatch( new LoginUserFailureAction( exception.Message ));
            }

            dispatcher.Dispatch( new ApiCallCompleted());
        }
    }
}
