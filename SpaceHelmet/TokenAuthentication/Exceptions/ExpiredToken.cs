using System;

namespace TokenAuthentication.Exceptions {
    public class ExpiredToken : Exception {
        public ExpiredToken( string message = "Token expired." ) : base( message ) {
        }
    }
}