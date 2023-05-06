using System.Collections.Generic;
using System.Linq;
using SpaceHelmet.Shared.Entities;
using Fluxor;
using SpaceHelmet.Client.Store;

namespace SpaceHelmet.Client.Users.Store {
    [FeatureState( CreateInitialStateMethodName = "Factory")]
    public class UserState : RootState {
        public IReadOnlyList<ShUser>    Users { get; }

        public UserState( bool callInProgress, string callMessage, 
            IEnumerable<ShUser> userList ) :
            base( callInProgress, callMessage ) {
            Users = new List<ShUser>( userList );
        }

        public static UserState Factory() => new ( false, string.Empty, Enumerable.Empty<ShUser>());
    }}
