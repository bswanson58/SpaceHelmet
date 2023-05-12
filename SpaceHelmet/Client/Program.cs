using System;
using System.Net.Http;
using Blazored.LocalStorage;
using FluentValidation;
using Fluxor;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;
using SpaceHelmet.Client;
using SpaceHelmet.Client.Auth.Store;
using SpaceHelmet.Client.Auth.Support;
using SpaceHelmet.Client.ClientApi;
using SpaceHelmet.Client.Constants;
using SpaceHelmet.Client.Gravatar;
using SpaceHelmet.Client.Support;
using SpaceHelmet.Client.Ui.Store;
using SpaceHelmet.Client.Ui;
using SpaceHelmet.Client.Users.Store;
using SpaceHelmet.Shared.Dto.Auth;

var builder = WebAssemblyHostBuilder.CreateDefault( args );

ConfigureRootComponents( builder.RootComponents );
ConfigureServices( builder.Services );

await builder.Build().RunAsync();


void ConfigureRootComponents( RootComponentMappingCollection root ) {
    root.Add<App>( "#app" );
    root.Add<HeadOutlet>( "head::after" );
}

void ConfigureServices( IServiceCollection services ) {
    services.AddHttpClient( HttpClientNames.Authenticated, 
                            client => client.BaseAddress = new Uri( builder.HostEnvironment.BaseAddress ))
        .AddHttpMessageHandler<JwtTokenHandler>()
        .AddPolicyHandler( RetryPolicyHandler.GetAuthorizedRetryPolicy());
    services.AddScoped( sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient( HttpClientNames.Authenticated ));
    services.AddScoped<JwtTokenHandler>();

    services.AddHttpClient( HttpClientNames.Anonymous,
                            client => client.BaseAddress = new Uri( builder.HostEnvironment.BaseAddress ))
        .AddPolicyHandler( RetryPolicyHandler.GetAnonymousRetryPolicy());

    services.AddScoped( sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient( HttpClientNames.Anonymous ));

    services.AddScoped<ITokenParser, PasetoParser>();
    services.AddScoped<ITokenRefresher, JwtTokenRefresher>();
    services.AddScoped<ITokenExpirationChecker, TokenExpirationChecker>();

    services.AddScoped<IResponseStatusHandler, ResponseStatusHandler>();
    services.AddScoped<IAuthenticatedHttpHandler, AuthenticatedHttpHandler>();
    services.AddScoped<IAnonymousHttpHandler, AnonymousHttpHandler>();

    services.AddScoped<IAppStartup, AppStartup>();
    services.AddScoped<AuthFacade>();
    services.AddScoped<UiFacade>();
    services.AddScoped<UserFacade>();

    services.AddScoped<AnnouncementHandler>();

    services.AddScoped<IGravatarClient, GravatarClient>();

    services.AddScoped<IDataRequester, DataRequester>();

    services.AddFluxor( options => options.ScanAssemblies( typeof( App ).Assembly ));

    services.AddValidatorsFromAssemblyContaining<CreateUserRequestValidator>();

    services.AddBlazoredLocalStorage();

    services.AddScoped<AuthenticationStateProvider, AuthStateProvider>();
    services.AddScoped<IAuthInformation, AuthInformation>();
    services.AddAuthorizationCore();

    services.AddMudServices();
}