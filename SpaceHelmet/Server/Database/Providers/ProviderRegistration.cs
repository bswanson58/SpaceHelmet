namespace SpaceHelmet.Server.Database.Providers {
    public static class ProviderRegistration {
        public static void AddEntityProviders( this IServiceCollection services ) {
            services.AddScoped<IUserProvider, TcUserProvider>();
        }
    }
}
