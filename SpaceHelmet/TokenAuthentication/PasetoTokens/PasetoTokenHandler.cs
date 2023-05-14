﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Paseto;
using Paseto.Builder;
using Paseto.Cryptography.Key;
using TokenAuthentication.Constants;
using TokenAuthentication.Exceptions;
using TokenAuthentication.Interfaces;
using TokenAuthentication.Models;
using TokenAuthentication.Settings;

namespace TokenAuthentication.PasetoTokens {
    public class PasetoTokenHandler : IPasetoTokenHandler {
        private readonly IAuthenticationSchemeProvider  mAuthenticationSchemeProvider;
        private readonly IRefreshTokenProvider          mRefreshTokenProvider;
        private readonly PasetoTokenOptions             mTokenOptions;

        public PasetoTokenHandler( IAuthenticationSchemeProvider authenticationSchemeProvider, IRefreshTokenProvider refreshTokenProvider,
                                   IOptions<PasetoTokenOptions> validationParameters ) {
            mAuthenticationSchemeProvider = authenticationSchemeProvider;
            mRefreshTokenProvider = refreshTokenProvider;
            mTokenOptions = validationParameters.Value;
        }

        public async Task<WebToken> WriteTokenAsync( PasetoTokenDescriptor descriptor, string footer = "" ) {
            var pasetoToken = new WebToken();
            var now = DateTime.Now;
            var expirationDate = descriptor.Expires ?? now + mTokenOptions.TokenExpiration;
            var audience = descriptor.Audience ?? mTokenOptions.Audience ?? String.Empty;
            var issuer = descriptor.Issuer ?? mTokenOptions.Issuer ?? String.Empty;

            var pasetoBuilder = new PasetoBuilder()
                .Use( ProtocolVersion.V4, Purpose.Local )
                .WithKey( Encoding.UTF8.GetBytes( mTokenOptions.SecretKey ), Encryption.SymmetricKey )
                .Audience( audience )
                .Issuer( issuer )
                .IssuedAt( now )
                .AddFooter( footer )
                .Expiration( expirationDate );

            if(!descriptor.NotBefore.Equals( null ) ) {
                pasetoBuilder.AddClaim( RegisteredClaims.NotBefore, descriptor.NotBefore );
            }

            foreach( Claim claim in descriptor.Subject.Claims ) {
                pasetoBuilder.AddClaim( claim.Type, claim.Value );
            }

            pasetoToken.Token = pasetoBuilder.Encode();
            pasetoToken.ExpiresAt = expirationDate;
            pasetoToken.RefreshToken =  await mRefreshTokenProvider.CreateAsync( descriptor.Subject );
            pasetoToken.RefreshExpiresAt = mRefreshTokenProvider.TokenExpiration();

            return pasetoToken;
        }

        public Task<PasetoAsymmetricKeyPair> GenerateKeyPairAsync( string secretKey ) {
            var bytes = Encoding.UTF8.GetBytes( secretKey );
            var keyPair = new PasetoBuilder()
                .Use( ProtocolVersion.V4, Purpose.Public )
                .GenerateAsymmetricKeyPair( bytes );

            return Task.FromResult( keyPair );
        }


        public async Task<ClaimsPrincipal> DecodeTokenAsync( string token ) {
            var valParams = new PasetoTokenValidationParameters {
                ValidateLifetime = false,
                ValidateAudience = false,
                ValidateIssuer = false
            };
            var decodedToken = new PasetoBuilder()
                .Use( ProtocolVersion.V4, Purpose.Local )
                .WithKey( Encoding.UTF8.GetBytes( mTokenOptions.SecretKey ), Encryption.SymmetricKey )
                .Decode( token, valParams );

            if( decodedToken.IsValid ) {
                if( Convert.ToDateTime( 
                        decodedToken.Paseto.Payload[PasetoRegisteredClaimsNames.ExpirationTime])
                        .CompareTo( DateTime.Now ) < 0 ||
                    Convert.ToDateTime( 
                        decodedToken.Paseto.Payload[PasetoRegisteredClaimsNames.NotBefore])
                        .CompareTo( DateTime.Now ) > 0 ) {
                    throw new ExpiredToken();
                }

                var claimsList = new List<Claim>();

                await Task.Run( () => {
                    foreach( var obj in decodedToken.Paseto.Payload ) {
                        switch( obj.Key ) {
                            case PasetoRegisteredClaimsNames.ExpirationTime:
                                claimsList.Add( 
                                    new Claim( PasetoRegisteredClaimsNames.ExpirationTime, obj.Value.ToString() ?? String.Empty ));
                                break;

                            case PasetoRegisteredClaimsNames.Audience:
                                claimsList.Add( 
                                    new Claim( PasetoRegisteredClaimsNames.Audience, obj.Value.ToString() ?? String.Empty ));
                                break;

                            case PasetoRegisteredClaimsNames.Issuer:
                                claimsList.Add(
                                    new Claim( PasetoRegisteredClaimsNames.Issuer, obj.Value.ToString() ?? String.Empty ));
                                break;

                            case PasetoRegisteredClaimsNames.IssuedAt:
                                claimsList.Add(
                                    new Claim( PasetoRegisteredClaimsNames.IssuedAt, obj.Value.ToString() ?? String.Empty ));
                                break;

                            case PasetoRegisteredClaimsNames.NotBefore:
                                claimsList.Add( 
                                    new Claim( PasetoRegisteredClaimsNames.NotBefore, obj.Value.ToString() ?? String.Empty ));
                                break;

                            case PasetoRegisteredClaimsNames.TokenIdentifier:
                                claimsList.Add( 
                                    new Claim( PasetoRegisteredClaimsNames.TokenIdentifier, obj.Value.ToString() ?? String.Empty ));
                                break;

                            case ClaimTypes.Role:
                                claimsList.AddRange( SplitRoleClaims( obj.Value.ToString()));
                                break;

                            default:
                                claimsList.Add( new Claim( obj.Key, obj.Value.ToString() ?? String.Empty ));
                                break;
                        }
                    }
                } );

                AuthenticationScheme ? authenticationScheme =
                    await mAuthenticationSchemeProvider.GetDefaultAuthenticateSchemeAsync();

                ClaimsIdentity identity = new ClaimsIdentity( claimsList, authenticationScheme?.Name );

                return new ClaimsPrincipal( identity );
            }

            return new ClaimsPrincipal( new ClaimsIdentity());
        }

        private static IEnumerable<Claim> SplitRoleClaims( string ? claimValues ) {
            if(!String.IsNullOrWhiteSpace( claimValues )) {
                var roles = claimValues.TrimStart( '[' ).TrimEnd( ']' ).Split( ',' );

                return roles.Select( r => new Claim( ClaimTypes.Role, r.Trim( '"' )));
            }

            return Enumerable.Empty<Claim>();
        }
    }
}