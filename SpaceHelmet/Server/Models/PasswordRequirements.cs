using Microsoft.AspNetCore.Identity;

namespace SpaceHelmet.Server.Models {
    public class PasswordRequirements {
        public  int     RequiredLength { get; set; }
        public  int     RequiredUniqueChars { get; set; }
        public  bool    RequireDigit { get; set; }
        public  bool    RequireNonAlphanumeric { get; set; }
        public  bool    RequireUppercase {  get; set; }
        public  bool    RequireLowercase {  get; set; }

        public PasswordRequirements() {
            // These would be the fallback values if a valid section in the app settings file is not loaded.
            RequiredLength = 8;
            RequiredUniqueChars = 1;
            RequireDigit = true;
            RequireNonAlphanumeric = false;
            RequireLowercase = true;
            RequireUppercase = true;
        }

        public static IdentityOptions LoadPasswordRequirements( IConfiguration configuration, IdentityOptions options ) {
            var passwordRequirements = 
                configuration.GetSection( "PasswordRequirements" ).Get<PasswordRequirements>() ?? new PasswordRequirements();

            options.Password.RequiredUniqueChars = passwordRequirements.RequiredUniqueChars;
            options.Password.RequireDigit = passwordRequirements.RequireDigit;
            options.Password.RequiredLength = passwordRequirements.RequiredLength;
            options.Password.RequireNonAlphanumeric = passwordRequirements.RequireNonAlphanumeric;
            options.Password.RequireUppercase = passwordRequirements.RequireUppercase;
            options.Password.RequireLowercase = passwordRequirements.RequireLowercase;
            options.User.RequireUniqueEmail = true;
            options.SignIn.RequireConfirmedAccount = false;
            options.SignIn.RequireConfirmedEmail = false;
            options.SignIn.RequireConfirmedPhoneNumber = false;

            return options;
        }
    }
}
