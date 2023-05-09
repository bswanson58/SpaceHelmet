using System;
using System.Net.Http.Headers;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PasetoAuth.Options;
using System.Linq;
using PasetoAuth.Exceptions;
using PasetoAuth.Interfaces;

namespace PasetoAuth {
    public class PasetoAuthHandler : AuthenticationHandler<PasetoValidationParameters> {
        private const string                    cAuthorizationHeaderName = "Authorization";
        private readonly IPasetoTokenHandler    mPasetoTokenHandler;

        public PasetoAuthHandler( IOptionsMonitor<PasetoValidationParameters> options,
                                  ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock,
                                  IPasetoTokenHandler pasetoTokenHandler )
            : base( options, logger, encoder, clock ) {
            mPasetoTokenHandler = pasetoTokenHandler;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync() {
            if(!Request.Headers.ContainsKey( cAuthorizationHeaderName )) {
                return AuthenticateResult.NoResult();
            }

            if(!AuthenticationHeaderValue.TryParse( Request.Headers[cAuthorizationHeaderName], 
                   out AuthenticationHeaderValue ? headerValue )) {
                return AuthenticateResult.NoResult();
            }

            if(!Scheme.Name.Equals( headerValue.Scheme, StringComparison.OrdinalIgnoreCase )) {
                return AuthenticateResult.NoResult();
            }

            try {
                var claimsPrincipal = await mPasetoTokenHandler.DecodeTokenAsync( headerValue.Parameter ?? String.Empty );

                if(!claimsPrincipal.Claims.Any()) {
                    throw new InvalidGrantType();
                }

                return AuthenticateResult.Success( new AuthenticationTicket( claimsPrincipal, Scheme.Name ));
            }
            catch( Exception ex ) {
                Response.Headers["Error-Message"] = ex.Message;

                return AuthenticateResult.Fail( ex );
            }
        }
    }
}