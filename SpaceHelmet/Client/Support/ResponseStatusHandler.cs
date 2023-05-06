using System;
using System.Net;
using System.Net.Http;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using SpaceHelmet.Client.Shared;
using SpaceHelmet.Client.Ui.Store;

namespace SpaceHelmet.Client.Support {
    public interface IResponseStatusHandler {
        void    HandleStatusCode( HttpResponseMessage message, string callerName );
        void    HandleCallFailure( string message );
        void    HandleException( Exception ex );
    }

    public class ResponseStatusHandler : IResponseStatusHandler {
        private readonly NavigationManager                  mNavigationManager;
        private readonly UiFacade                           mUiFacade;
        private readonly ILogger<ResponseStatusHandler>     mLogger;

        public ResponseStatusHandler( NavigationManager navigationManager,
                                      ILogger<ResponseStatusHandler> logger, UiFacade uiFacade ) {
            mLogger = logger;
            mUiFacade = uiFacade;
            mNavigationManager = navigationManager;
        }

        public void HandleStatusCode( HttpResponseMessage message, string callerName ) {
            if( message.StatusCode is 
               HttpStatusCode.NoContent or 
               HttpStatusCode.Accepted or 
               HttpStatusCode.OK ) {
                return;
            }

            if( message.StatusCode is HttpStatusCode.Unauthorized ) {
                mUiFacade.ApiCallFailure( $"Unauthorized request, please login. ({callerName})" );

                mNavigationManager.NavigateTo( NavLinks.Login );
            }

            if(!String.IsNullOrWhiteSpace( message.ReasonPhrase )) {
                mUiFacade.ApiCallFailure( $"{message.ReasonPhrase} ({callerName})" );
            }
        }

        public void HandleCallFailure( string message ) {
            mUiFacade.ApiCallFailure( message );
        }

        public void HandleException( Exception ex ) {
            mLogger.LogError( ex, String.Empty );

            mUiFacade.ApiCallFailure( $"Error: {ex.Message}" );
        }
    }
}
