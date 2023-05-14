using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using SpaceHelmet.Shared.Dto;
using MudBlazor;
using SpaceHelmet.Client.Auth.Store;
using SpaceHelmet.Client.Auth.Support;
using SpaceHelmet.Client.Constants;
using TokenClientSupport.Constants;

namespace SpaceHelmet.Client.Support {
    public interface IAuthenticatedHttpHandler : IBaseHttpHandler { }
    public interface IAnonymousHttpHandler: IBaseHttpHandler { }

    public interface IBaseHttpHandler {
        Task<TResponse ?>           Get<TResponse>( string route,
                                                   [CallerMemberName] string callerName = FilterOperator.String.Empty );
        Task<TResponse ?>           Post<TResponse>( string route, object request,
                                                    [CallerMemberName] string callerName = FilterOperator.String.Empty );
        Task<HttpResponseMessage ?> Post( string route, object request,
                                         [CallerMemberName] string callerName = FilterOperator.String.Empty );
    }

    public class AuthenticatedHttpHandler : BaseHttpHandler, IAuthenticatedHttpHandler {
        public AuthenticatedHttpHandler( IHttpClientFactory clientFactory, IResponseStatusHandler statusHandler,
                                         IAuthInformation authInformation, AuthFacade authFacade )
            : base( clientFactory, statusHandler, authInformation, authFacade ) { }

        protected override string   ClientName => HttpClientNames.Authenticated;
    }

    public class AnonymousHttpHandler : BaseHttpHandler, IAnonymousHttpHandler {
        public AnonymousHttpHandler( IHttpClientFactory clientFactory, IResponseStatusHandler statusHandler,
                                     IAuthInformation authInformation, AuthFacade authFacade )
            : base( clientFactory, statusHandler, authInformation, authFacade ) { }

        protected override string   ClientName => HttpClientNames.Anonymous;
    }

    public abstract class BaseHttpHandler : IBaseHttpHandler {
        private readonly IHttpClientFactory     mClientFactory;
        private readonly IResponseStatusHandler mStatusHandler;
        private readonly IAuthInformation       mAuthInformation;
        private readonly AuthFacade             mAuthFacade;

        protected abstract string               ClientName { get; }

        protected BaseHttpHandler( IHttpClientFactory clientFactory, IResponseStatusHandler statusHandler,
                                   IAuthInformation authInformation, AuthFacade authFacade ) {
            mClientFactory = clientFactory;
            mStatusHandler = statusHandler;
            mAuthInformation = authInformation;
            mAuthFacade = authFacade;
        }

        public async Task<TResponse ?> Get<TResponse>( string route,
                                                      [CallerMemberName] string callerName = FilterOperator.String.Empty ) {
            try {
                using var httpClient = mClientFactory.CreateClient( ClientName );

                return await HandleResponse<TResponse>( await httpClient.GetAsync( route ), callerName );
            }
            catch( Exception ex ) {
                mStatusHandler.HandleException( ex );

                return default;
            }
        }

        public async Task<TResponse ?> Post<TResponse>( string route, object request,
                                                       [CallerMemberName] string callerName = FilterOperator.String.Empty ) {
            try {
                using var httpClient = mClientFactory.CreateClient( ClientName );

                return await HandleResponse<TResponse>( await httpClient.PostAsJsonAsync( route, request ), callerName );
            }
            catch( Exception ex ) {
                mStatusHandler.HandleException( ex );

                return default;
            }
        }

        public async Task<HttpResponseMessage ?> Post( string route, object request,
                                                      [CallerMemberName] string callerName = FilterOperator.String.Empty ) {
            try {
                using var httpClient = mClientFactory.CreateClient( ClientName );

                var response = await httpClient.PostAsJsonAsync( route, request );

                if(!response.IsSuccessStatusCode ) {
                    mStatusHandler.HandleStatusCode( response, callerName );
                }

                return response;
            }
            catch( Exception ex ) {
                mStatusHandler.HandleException( ex );

                return default;
            }
        }

        private async Task<TResponse ?> HandleResponse<TResponse>( HttpResponseMessage response, string callerName ) {
            if( response.StatusCode == HttpStatusCode.NoContent ) {
                return default;
            }

            if(!response.IsSuccessStatusCode ) {
                mStatusHandler.HandleStatusCode( response, callerName );

                return default;
            }

            // check to see if the authentication token has been updated during the call.
            IEnumerable<string> ? authValue = default;
            IEnumerable<string> ? refreshValue = default;
            IEnumerable<string> ? expirationValue = default;

            response.RequestMessage?.Headers.TryGetValues( TokenStorageNames.AuthToken, out authValue );
            response.RequestMessage?.Headers.TryGetValues( TokenStorageNames.RefreshToken, out refreshValue );
            response.RequestMessage?.Headers.TryGetValues( TokenStorageNames.TokenExpiration, out expirationValue );

            if(( authValue != null ) &&
               ( refreshValue != null ) &&
               ( expirationValue != null )) {
                var authToken = authValue.FirstOrDefault();
                var refreshToken = refreshValue.FirstOrDefault();
                var expiration = expirationValue.FirstOrDefault();

                if((!String.IsNullOrWhiteSpace( authToken )) &&
                   (!String.IsNullOrWhiteSpace( refreshToken )) &&
                   (!String.IsNullOrWhiteSpace( expiration )) &&
                   (!mAuthInformation.UserToken.Equals( authToken ))) {
                    if( Int64.TryParse( expiration, out var expirationTicks )) {
                        mAuthFacade.SetRefreshToken( authToken, refreshToken, DateTime.FromBinary( expirationTicks ));
                    }
                }
            }

            var retValue = await response.Content.ReadFromJsonAsync<TResponse>();

            if( retValue is BaseResponse { Succeeded: false } message ) {
                mStatusHandler.HandleCallFailure( $"{message.Message} ({callerName})" );

                return default;
            }

            return retValue;
        }
    }
}
