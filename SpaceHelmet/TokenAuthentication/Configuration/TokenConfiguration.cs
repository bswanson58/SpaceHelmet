using Microsoft.Extensions.DependencyInjection;
using System;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using TokenAuthentication.Constants;
using TokenAuthentication.Interfaces;
using TokenAuthentication.JsonTokens;
using TokenAuthentication.PasetoTokens;
using TokenAuthentication.RefreshTokens;
using TokenAuthentication.Settings;

namespace TokenAuthentication.Configuration {
    public static class TokenConfiguration {
        public static void AddTokenConfiguration( this IServiceCollection services, ConfigurationManager configuration ) {
            var tokenOptions = configuration
                                   .GetSection( nameof( TokenOptions ))
                                   .Get<TokenOptions>() ??
                               new TokenOptions();

            if( tokenOptions.UseTokens ) {
                switch( tokenOptions.TokenStyle.ToLower() ) {
                    case TokenStyles.JsonWebTokens:
                        AddJwtTokens( services, configuration );
                        break;

                    case TokenStyles.PasetoTokens:
                        AddPasetoTokens( services, configuration );
                        break;

                    default:
                        throw new ApplicationException( "Invalid token configuration " );
                }
            }

            services.AddScoped<IRefreshTokenProvider, RefreshTokenProvider>();
        }

        private static void AddPasetoTokens( IServiceCollection services, ConfigurationManager configuration ) {
            var pasetoOptions = configuration
                                    .GetSection( nameof( PasetoValidationParameters ))
                                    .Get<PasetoValidationParameters>() ??
                                new PasetoValidationParameters();

            services.AddAuthentication( options => {
                options.DefaultChallengeScheme = PasetoDefaults.Bearer;
                options.DefaultAuthenticateScheme = PasetoDefaults.Bearer;
            } ).AddPaseto( options => {
                options.Audience = pasetoOptions.Audience;
                options.DefaultExpirationTime = pasetoOptions.DefaultExpirationTime;
                options.Issuer = pasetoOptions.Issuer;
                options.ClockSkew = pasetoOptions.ClockSkew;
                options.SecretKey = pasetoOptions.SecretKey;
                options.UseRefreshToken = pasetoOptions.UseRefreshToken;
                options.ValidateAudience = pasetoOptions.ValidateAudience;
                options.ValidateIssuer = pasetoOptions.ValidateIssuer;
                options.UseRefreshToken = true;
            } );

            services.AddScoped<ITokenBuilder, PasetoTokenBuilder>();
        }

        private static void AddJwtTokens( IServiceCollection services, ConfigurationManager configuration ) {
            var jwtSettings = configuration.GetSection( JwtConstants.JwtConfigSettings );

            services.AddAuthentication( options => {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            } )
                .AddJwtBearer( options => {
                    options.TokenValidationParameters = JwtTokenBuilder.CreateTokenValidationParameters( jwtSettings );
                } );

            services.AddScoped<ITokenBuilder, JwtTokenBuilder>();
        }
    }
}
