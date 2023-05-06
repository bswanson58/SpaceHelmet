using System.Net.Http;
using System.Threading.Tasks;
using Fluxor;
using SpaceHelmet.Client.Auth.Actions;
using SpaceHelmet.Client.Auth.Store;

namespace SpaceHelmet.Client.Auth.Effects {
    // ReSharper disable once UnusedType.Global
    public class LogoutUserEffect : Effect<LogoutUserAction> {
        private readonly HttpClient     mHttpClient;
        private readonly AuthFacade     mAuthFacade;
        private readonly IAppStartup    mAppStartup;

        public LogoutUserEffect( HttpClient httpClient, AuthFacade authFacade, IAppStartup appStartup ) {
            mHttpClient = httpClient;
            mAuthFacade = authFacade;
            mAppStartup = appStartup;
        }

        public override Task HandleAsync( LogoutUserAction action, IDispatcher dispatcher ) {
            mAuthFacade.ClearAuthenticationToken();
            mAppStartup.OnLogout();

            mHttpClient.DefaultRequestHeaders.Authorization = null;

            return Task.CompletedTask;
        }
    }
}
