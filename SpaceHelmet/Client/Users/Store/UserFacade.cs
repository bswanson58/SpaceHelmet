using Fluxor;
using SpaceHelmet.Client.Users.Actions;

namespace SpaceHelmet.Client.Users.Store {
    public class UserFacade {
        private readonly IDispatcher    mDispatcher;

        public UserFacade( IDispatcher dispatcher ) {
            mDispatcher = dispatcher;
        }

        public void LoadUsers() {
            mDispatcher.Dispatch( new GetUsersAction());
        }
    }
}
