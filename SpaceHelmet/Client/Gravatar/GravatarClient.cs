using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using SpaceHelmet.Client.Gravatar.Models;

// ReSharper disable StringLiteralTypo
// ReSharper disable IdentifierTypo

// info at: https://en.gravatar.com/site/implement/

namespace SpaceHelmet.Client.Gravatar {
    public enum GravatarDefaultImage {
        None,
        MysteryPerson,
        IdentIcon,
        MonsterId,
        Wavatar,
        Retro,
        RoboHash,
        Blank
    }

    public interface IGravatarClient {
        Task<Entry>         GetProfile( string email );

        Uri                 GetImageUri( string emailHash, GravatarDefaultImage defaultImage, bool forceDefault, uint imageSize );
        Uri                 GetImageUri( string email );
        Uri                 GetImageUri( string email, uint imageSize );
        Uri                 GetImageUri( string email, GravatarDefaultImage imageStyle, bool forceDefault );

        Task<MemoryStream>  GetImage( string email );
        Task<MemoryStream>  GetImage( string email, uint imageSize );
        Task<MemoryStream>  GetImage( string email, GravatarDefaultImage imageStyle, bool forceDefault );
        Task<MemoryStream>  GetImage( string email, GravatarDefaultImage imageStyle, bool forceDefault, uint imageSize );
    }

    public class GravatarClient : IGravatarClient, IDisposable {
        private const string        cGravatarUri = "https://en.gravatar.com/";
        private readonly HttpClient mHttpClient;

        public GravatarClient( HttpClient httpClient ) {
            mHttpClient = httpClient;
            mHttpClient.DefaultRequestHeaders.UserAgent.ParseAdd( "SecretSquirrelSoftware/1.0" );
        }

        public async Task<Entry> GetProfile( string emailHash ) {
            if( String.IsNullOrWhiteSpace( emailHash )) {
                throw new ArgumentException( "Email must be provided", nameof( emailHash ));
            }

            var uri = new Uri( $"{cGravatarUri}{emailHash}.json" );
            var response = await mHttpClient.GetFromJsonAsync<GravatarResponse>( uri ).ConfigureAwait( false );
            var entry = response?.Entry.FirstOrDefault();

            return entry ?? new Entry();
        }

        // ReSharper disable once CyclomaticComplexity
        private static string ImageParameters( GravatarDefaultImage style, bool forceDefault, uint imageSize ) {
            var sizeParam = imageSize > 0 ? $"s={imageSize}" : String.Empty;

            if( style == GravatarDefaultImage.None ) {
                return $"?{sizeParam}";
            }

            var forceParam = forceDefault ? "&f=y" : String.Empty;
            var defaultParam = style switch {
                GravatarDefaultImage.Blank => "blank",
                GravatarDefaultImage.IdentIcon => "identicon",
                GravatarDefaultImage.MonsterId => "monsterid",
                GravatarDefaultImage.MysteryPerson => "mp",
                GravatarDefaultImage.Retro => "retro",
                GravatarDefaultImage.RoboHash => "robohash",
                GravatarDefaultImage.Wavatar => "wavatar",
                _ => String.Empty
            };

            return $"?{sizeParam}&d={defaultParam}{forceParam}";
        }

        public Task<MemoryStream> GetImage( string emailHash ) => 
            GetImage( emailHash, GravatarDefaultImage.None, false, 0 );
        public Task<MemoryStream> GetImage( string emailHash, uint imageSize ) => 
            GetImage( emailHash, GravatarDefaultImage.None, false, imageSize );
        public Task<MemoryStream> GetImage( string emailHash, GravatarDefaultImage imageStyle, bool forceDefault ) => 
            GetImage( emailHash, imageStyle, forceDefault, 0 );

        public async Task<MemoryStream> GetImage( string emailHash, GravatarDefaultImage defaultImage, 
                                                  bool forceDefault, uint imageSize ) {
            if( String.IsNullOrWhiteSpace( emailHash )) {
                throw new ArgumentException( "Email hash must be provided", nameof( emailHash ));
            }

            var retValue = new MemoryStream();
            var uri = GetImageUri( emailHash, defaultImage, forceDefault, imageSize );
            using var response = await mHttpClient.GetAsync( uri ).ConfigureAwait( false );

            if( response.IsSuccessStatusCode ) {
                await using var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait( false );

                await stream.CopyToAsync( retValue, 8192 ).ConfigureAwait( false );
            }

            return retValue;
        }

        public Uri GetImageUri( string emailHash ) => 
            GetImageUri( emailHash, GravatarDefaultImage.None, false, 0 );
        public Uri GetImageUri( string emailHash, uint imageSize ) => 
            GetImageUri( emailHash, GravatarDefaultImage.None, false, imageSize );
        public Uri GetImageUri( string emailHash, GravatarDefaultImage imageStyle, bool forceDefault ) => 
            GetImageUri( emailHash, imageStyle, forceDefault, 0 );

        public Uri GetImageUri( string emailHash, GravatarDefaultImage defaultImage, 
                                bool forceDefault, uint imageSize ) {
            if( String.IsNullOrWhiteSpace( emailHash )) {
                throw new ArgumentException( "Email hash must be provided", nameof( emailHash ));
            }

            return new Uri( $"{cGravatarUri}avatar/{emailHash}{ImageParameters( defaultImage, forceDefault, imageSize )}");
        }

        public void Dispose() {
            mHttpClient.Dispose();
        }
    }
}
