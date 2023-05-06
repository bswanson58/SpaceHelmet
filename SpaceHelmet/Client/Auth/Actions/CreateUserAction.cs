using SpaceHelmet.Shared.Dto.Auth;
using SpaceHelmet.Client.Store;

namespace SpaceHelmet.Client.Auth.Actions {
    public class CreateUserAction {
    }

    public class CreateUserSubmitAction {
        public  CreateUserRequest   UserInput {  get; }

        public CreateUserSubmitAction( CreateUserRequest userInput ) {
            UserInput = userInput;
        }
    }

    public class CreateUserSuccessAction {
    }

    public class CreateUserFailureAction : FailureAction {
        public CreateUserFailureAction( string message ) :
            base( message ) { }
    }
}
