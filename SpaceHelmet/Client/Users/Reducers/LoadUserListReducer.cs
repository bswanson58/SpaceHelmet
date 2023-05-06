using System;
using Fluxor;
using SpaceHelmet.Client.Users.Actions;
using SpaceHelmet.Client.Users.Store;

namespace SpaceHelmet.Client.Users.Reducers {
    // ReSharper disable once UnusedType.Global
    public static class LoadUserListReducer {
        [ReducerMethod( typeof( GetUsersAction ))]
        public static UserState GetUserList( UserState state ) =>
            new ( true, String.Empty, state.Users );

        [ReducerMethod]
        public static UserState GetUsersSuccess( UserState _, GetUsersSuccessAction action ) =>
            new ( false, String.Empty, action.Users );

        [ReducerMethod]
        public static UserState GetUsersFailure( UserState state, GetUsersFailureAction action ) =>
            new ( false, action.Message, state.Users );
    }
}
