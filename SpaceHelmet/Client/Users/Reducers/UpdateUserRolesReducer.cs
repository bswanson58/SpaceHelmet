using System;
using System.Collections.Generic;
using System.Linq;
using SpaceHelmet.Shared.Entities;
using Fluxor;
using SpaceHelmet.Client.Auth.Actions;
using SpaceHelmet.Client.Users.Store;

namespace SpaceHelmet.Client.Users.Reducers {
    // ReSharper disable once UnusedType.Global
    public static class UpdateUserRolesReducer {
        [ReducerMethod]
        public static UserState ChangeUserRoles( UserState state, ChangeUserRolesSuccess action ) {
            var users = new List<ShUser>();

            users.AddRange( state.Users.Select( u => u.EntityId.Equals( action.User.EntityId ) ? action.User : u ));

            return new( false, String.Empty, users );
        }
    }
}
