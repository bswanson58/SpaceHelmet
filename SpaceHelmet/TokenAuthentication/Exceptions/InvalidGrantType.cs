using System;

namespace TokenAuthentication.Exceptions {
    public class InvalidGrantType : Exception {
        public InvalidGrantType() : base( "This grant type is unsupported" ) {
        }
    }
}