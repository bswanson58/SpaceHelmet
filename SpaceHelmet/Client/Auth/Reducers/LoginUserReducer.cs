﻿using System;
using SpaceHelmet.Shared.Support;
using Fluxor;
using SpaceHelmet.Client.Auth.Actions;
using SpaceHelmet.Client.Auth.Store;

namespace SpaceHelmet.Client.Auth.Reducers {
    // ReSharper disable once UnusedType.Global
    public static class LoginUserReducer {
        [ReducerMethod]
        public static AuthState SetAuthTokenReducer( AuthState state, SetAuthToken action ) =>
            new ( false, String.Empty, action.Token, action.RefreshToken, state.TokenExpiration );

        [ReducerMethod( typeof( LoginUserSubmitAction ))]
        public static AuthState CreateUserSubmitReducer( AuthState state ) =>
            new ( true, String.Empty, state.UserToken, state.RefreshToken, state.TokenExpiration );

        [ReducerMethod( typeof( LoginUserSuccessAction ))]
        public static AuthState LoginUserSuccessReducer( AuthState state ) =>
            new ( false, String.Empty, state.UserToken, state.RefreshToken, state.TokenExpiration );

        [ReducerMethod]
        public static AuthState LoginUserFailureReducer( AuthState state, LoginUserFailureAction action ) =>
            new ( false, action.Message, String.Empty, String.Empty, DateTimeProvider.Instance.CurrentDateTime );

        [ReducerMethod( typeof( LogoutUserAction ))]
        public static AuthState LogoutUserReducer( AuthState state ) =>
            new( false, String.Empty, String.Empty, String.Empty, DateTimeProvider.Instance.CurrentDateTime );
    }
}
