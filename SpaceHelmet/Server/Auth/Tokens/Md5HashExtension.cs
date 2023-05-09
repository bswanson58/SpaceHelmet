using System.Security.Cryptography;
using System.Text;

// warning about using md5 version of compute hash
#pragma warning disable CA1850

namespace SpaceHelmet.Server.Auth.Tokens {
    public static class Md5HashExtension {
        public static string CalculateMd5Hash( this string input ) {
            if( string.IsNullOrWhiteSpace( input )) {
                return string.Empty;
            }

            // step 1, calculate MD5 hash from input
            var md5 = MD5.Create();
            var inputBytes = Encoding.ASCII.GetBytes(input);
            var hash = md5.ComputeHash(inputBytes);

            // step 2, convert byte array to hex string
            var sb = new StringBuilder();

            foreach( var t in hash ) {
                sb.Append( t.ToString( "X2" ));
            }

            return sb.ToString().ToLower();
        }
    }
}
