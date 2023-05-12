using System;
using System.Threading;
using System.Threading.Tasks;
using SpaceHelmet.Client.Support;
using SpaceHelmet.Client.Ui.Store;
using SpaceHelmet.Shared.Dto.ClientApi;

namespace SpaceHelmet.Client.ClientApi {
    public interface IDataRequester : IDisposable {
        void    StartRequesting();
        void    StopRequesting();
    }

    public class DataRequester : IDataRequester {
        private const int                           cCheckTimeInSeconds = 30;

        private readonly UiFacade                   mUiFacade;
        private readonly IAuthenticatedHttpHandler  mHttpHandler;
        private Timer ?                             mTimer;

        public DataRequester( IAuthenticatedHttpHandler httpHandler, UiFacade uiFacade ) {
            mHttpHandler = httpHandler;
            mUiFacade = uiFacade;
            mTimer = null;
        }

        public void StartRequesting() {
            StopRequesting();

            mTimer = new Timer( OnTimer, null, 
                TimeSpan.FromSeconds( cCheckTimeInSeconds ), 
                TimeSpan.FromSeconds( cCheckTimeInSeconds ));
        }

        private void OnTimer( object ? state ) {
            RequestData();
        }

        private async void RequestData() {
            mUiFacade.ApiCallStarted( "DataRequest" );

            var request = new ClientRequest( "data needed" );
            var response = await mHttpHandler.Post<ClientResponse>( ClientRequest.Route, request );


            if( response?.Succeeded == true ) {
                await  Task.Delay( TimeSpan.FromSeconds( 5 ));

                mUiFacade.ApiCallCompleted();
            }
            else {
                mUiFacade.ApiCallFailure( response?.Message ?? "Oops!" );
            }
        }

        public void StopRequesting() {
            mTimer?.Dispose();
            mTimer = null;
        }

        public void Dispose() {
            StopRequesting();
        }
    }
}
