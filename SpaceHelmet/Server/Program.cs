using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SpaceHelmet.Server.Auth;
using SpaceHelmet.Server.Database;
using SpaceHelmet.Server.Database.Entities;
using SpaceHelmet.Server.Database.Providers;
using SpaceHelmet.Server.Models;
using SpaceHelmet.Shared.Constants;
using SpaceHelmet.Shared.Dto.Auth;
using TokenAuthentication.Configuration;
using TokenAuthentication.Interfaces;

var builder = WebApplication.CreateBuilder( args );

ConfigureServices( builder.Services, builder.Configuration );
ConfigureSecurity( builder.Services, builder.Configuration );

var app = builder.Build();

ConfigurePipeline( app );

app.Run();

void ConfigureServices( IServiceCollection services, ConfigurationManager configuration ) {
    services.AddControllers();

    services.AddDbContext<SpaceHelmetDbContext>( options => {
        options.UseSqlServer( configuration.GetConnectionString( "DatabaseConnection" ), sqlOptions => {
            sqlOptions.EnableRetryOnFailure(  
                maxRetryCount: 3,
                maxRetryDelay: TimeSpan.FromSeconds( 3 ),
                errorNumbersToAdd: null );
        });
        
#if DEBUG
        options.EnableDetailedErrors();
        options.EnableSensitiveDataLogging();
#endif
    });

    services.AddEntityProviders();
    services.AddScoped<IUserService, UserService>();

    services.AddValidatorsFromAssemblyContaining<LoginUserRequestValidator>();
}

void ConfigureSecurity( IServiceCollection services, ConfigurationManager configuration ) {
    // AddIdentity must be called before AddAuthentication
    services.AddIdentity<DbUser, IdentityRole>( 
            options => PasswordRequirements.LoadPasswordRequirements( configuration, options ))
        .AddEntityFrameworkStores<SpaceHelmetDbContext>()
        .AddDefaultTokenProviders();

    services.AddAuthorization( auth => {
        auth.AddPolicy( ClaimValues.cAdministrator, policy => policy.RequireRole( ClaimValues.cAdministrator ));
        auth.AddPolicy( ClaimValues.cUser, policy => policy.RequireRole( ClaimValues.cUser ));
    });

    services.AddTokenConfiguration( configuration );
    services.AddScoped<IClaimBuilder, ClaimBuilder>();
}

void ConfigurePipeline( WebApplication webApp ) {
    if( webApp.Environment.IsDevelopment()) {
        webApp.UseWebAssemblyDebugging();
    }
    else {
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        webApp.UseHsts();
    }

    webApp.UseHttpsRedirection();
    webApp.UseBlazorFrameworkFiles();
    webApp.UseStaticFiles();

    webApp.UseRouting();

    webApp.MapControllers();
    webApp.MapFallbackToFile( "index.html" );

    webApp.UseAuthentication();
    webApp.UseAuthorization();
}
