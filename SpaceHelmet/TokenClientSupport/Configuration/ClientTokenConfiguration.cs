using System;
using System.IO;
using System.Net.Http;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TokenClientSupport.Constants;
using TokenClientSupport.Interfaces;
using TokenClientSupport.JsonTokens;
using TokenClientSupport.PasetoTokens;
using TokenClientSupport.RefreshTokens;
using TokenClientSupport.Settings;
using TokenClientSupport.Support;

namespace TokenClientSupport.Configuration {
    public static class ClientTokenConfiguration {
        public static void AddTokenConfiguration( this IServiceCollection services, 
                                                  WebAssemblyHostBuilder builder ) {
            var tokenOptions = builder.Configuration
                .GetSection( nameof( ClientTokenOptions ))
                .Get<ClientTokenOptions>() ??
                          new ClientTokenOptions();

            if( tokenOptions.UseTokens ) {
                switch( tokenOptions.TokenStyle.ToLower() ) {
                    case TokenStyles.JsonWebTokens:
                        ConfigureJsonTokens( services );
                        break;

                    case TokenStyles.PasetoTokens:
                        ConfigurePasetoTokens( services );
                        break;

                    default:
                        throw new ApplicationException( "Invalid token configuration " );
                }
            }

            services.AddScoped<TokenHandler>();
            services.AddScoped<ITokenRefresher, TokenRefresher>();
            services.AddSingleton<ITokenStorageProvider, SimpleTokenProvider>();

            var serverRoute = String.IsNullOrWhiteSpace( tokenOptions.BaseRoute ) ?
                                    builder.HostEnvironment.BaseAddress :
                                    Path.Combine( Path.Combine( builder.HostEnvironment.BaseAddress, tokenOptions.BaseRoute ));
            if(!serverRoute.EndsWith( "/" )) {
                serverRoute = $"{serverRoute}/";
            }

            services.AddHttpClient( TokenClientNames.RefreshClient,
                    client => client.BaseAddress = new Uri( serverRoute ))
                .AddPolicyHandler( RefreshPolicyHandler.GetRefreshRetryPolicy());

            services.AddScoped( sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient( TokenClientNames.RefreshClient ));
        }

        private static void ConfigureJsonTokens( IServiceCollection services ) {
            services.AddScoped<ITokenParser, JwtParser>();
        }

        private static void ConfigurePasetoTokens( IServiceCollection services ) {
            services.AddScoped<ITokenParser, PasetoParser>();
        }
    }
}
