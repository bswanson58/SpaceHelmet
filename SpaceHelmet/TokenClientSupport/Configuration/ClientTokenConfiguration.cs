using System;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TokenClientSupport.Interfaces;
using TokenClientSupport.JsonTokens;
using TokenClientSupport.PasetoTokens;
using TokenClientSupport.Settings;

namespace TokenClientSupport.Configuration {
    public static class ClientTokenConfiguration {
        public static void AddTokenConfiguration( this IServiceCollection services, WebAssemblyHostConfiguration configuration ) {
            var tokenOptions = configuration
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
        }

        private static void ConfigureJsonTokens( IServiceCollection services ) {
            services.AddScoped<ITokenParser, JwtParser>();

        }

        private static void ConfigurePasetoTokens( IServiceCollection services ) {
            services.AddScoped<ITokenParser, PasetoParser>();

        }
    }
}
