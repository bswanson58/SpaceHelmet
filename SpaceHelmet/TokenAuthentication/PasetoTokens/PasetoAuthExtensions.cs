﻿using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using PasetoAuth.Options;
using TokenAuthentication.Constants;
using TokenAuthentication.Interfaces;
using TokenAuthentication.Settings;

namespace TokenAuthentication.PasetoTokens {
    public static class PasetoAuthExtensions {

        public static AuthenticationBuilder AddPaseto( this AuthenticationBuilder builder,
            Action<PasetoValidationParameters> configureOptions ) {
            return AddPaseto( builder, PasetoDefaults.Bearer, configureOptions );
        }

        public static AuthenticationBuilder AddPaseto( this AuthenticationBuilder builder,
            string authenticationScheme,
            Action<PasetoValidationParameters> configureOptions ) {
            builder.Services.AddScoped<IPasetoTokenHandler, PasetoTokenHandler>();
            builder.Services.Configure( configureOptions );
            builder.Services.AddSingleton<IPostConfigureOptions<PasetoValidationParameters>, PasetoValidationParametersPostConfigure>();

            return builder.AddScheme<PasetoValidationParameters, PasetoAuthHandler>( authenticationScheme, configureOptions );
        }
    }
}
