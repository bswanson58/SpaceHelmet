using System;
using Microsoft.Extensions.Options;
using TokenAuthentication.Settings;

namespace PasetoAuth.Options {
    public class PasetoTokenOptionsPostConfigure : IPostConfigureOptions<PasetoTokenOptions> {
        public void PostConfigure( string ? name, PasetoTokenOptions options ) {
            if( string.IsNullOrEmpty( options.SecretKey )) {
                throw new InvalidOperationException( "Secret key is required." );
            }

            if( options.SecretKey.Length != 32 ) {
                throw new InvalidOperationException( "Secret key must have 32 chars." );
            }
        }
    }
}