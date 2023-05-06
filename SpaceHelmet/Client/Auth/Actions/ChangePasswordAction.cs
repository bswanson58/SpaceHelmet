using SpaceHelmet.Shared.Dto.Auth;
using SpaceHelmet.Client.Store;

namespace SpaceHelmet.Client.Auth.Actions {
    public class ChangePasswordAction {
        public  ChangePasswordRequest   Request { get; }

        public ChangePasswordAction() {
            Request = new ChangePasswordRequest();
        }
    }

    public class ChangePasswordSubmit {
        public  ChangePasswordRequest   Request { get; }

        public ChangePasswordSubmit( ChangePasswordRequest request ) {
            Request = request;
        }
    }

    public class ChangePasswordSuccess { }

    public class ChangePasswordFailure : FailureAction {
        public ChangePasswordFailure( string message )
            : base( message ) { }
    }
}
