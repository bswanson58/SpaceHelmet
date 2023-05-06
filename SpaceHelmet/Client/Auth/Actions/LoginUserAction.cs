using SpaceHelmet.Shared.Dto.Auth;
using SpaceHelmet.Client.Store;

namespace SpaceHelmet.Client.Auth.Actions {
    public class LoginUserAction {
    }

    public class LoginUserSubmitAction {
        public  LoginUserRequest    UserInput {  get; }

        public LoginUserSubmitAction( LoginUserRequest userInput ) {
            UserInput = userInput;
        }
    }

    public class LoginUserSuccessAction {
        public LoginUserResponse    UserResponse { get; }

        public LoginUserSuccessAction( LoginUserResponse response ) {
            UserResponse = response;
        }
    }

    public class LoginUserFailureAction : FailureAction {
        public LoginUserFailureAction( string message ) :
            base( message ) { }
    }
}
