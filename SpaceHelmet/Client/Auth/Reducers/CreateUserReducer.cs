using System;
using Fluxor;
using SpaceHelmet.Client.Auth.Actions;
using SpaceHelmet.Client.Auth.Store;

namespace SpaceHelmet.Client.Auth.Reducers {
    // ReSharper disable once UnusedType.Global
    public static class CreateUserReducer {
        [ReducerMethod( typeof( CreateUserSubmitAction ))]
        public static AuthState CreateUserSubmitReducer( AuthState state ) =>
            new ( true, String.Empty, state.UserToken, state.RefreshToken, state.TokenExpiration );

        [ReducerMethod( typeof( CreateUserSuccessAction ))]
        public static AuthState CreateUserSuccessReducer( AuthState state ) =>
            new ( false, String.Empty, state.UserToken, state.RefreshToken, state.TokenExpiration );

        [ReducerMethod]
        public static AuthState CreateUserFailureReducer( AuthState state, CreateUserFailureAction action ) =>
            new ( false, action.Message, state.UserToken, state.RefreshToken, state.TokenExpiration );
    }
}
