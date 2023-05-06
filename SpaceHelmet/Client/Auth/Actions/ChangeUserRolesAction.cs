using SpaceHelmet.Shared.Dto.Auth;
using SpaceHelmet.Shared.Entities;
using SpaceHelmet.Client.Store;

namespace SpaceHelmet.Client.Auth.Actions {
    public class ChangeUserRolesAction {
        public  ShUser  User { get; }

        public ChangeUserRolesAction( ShUser user ) {
            User = user;
        }
    }

    public class ChangeUserRolesSubmit {
        public  ChangeUserRolesRequest  Request { get; }

        public ChangeUserRolesSubmit( ChangeUserRolesRequest request ) {
            Request = request;
        }
    }

    public class ChangeUserRolesSuccess {
        public  ShUser  User { get; }

        public ChangeUserRolesSuccess( ShUser user ) {
            User = user;
        }
    }

    public class ChangeUserRolesFailure : FailureAction {
        public ChangeUserRolesFailure( string message )
            : base( message ) { }
    }
}
