using System.Collections.Generic;
using SpaceHelmet.Shared.Dto;
using SpaceHelmet.Shared.Entities;
using SpaceHelmet.Client.Store;

namespace SpaceHelmet.Client.Users.Actions {
    public class GetUsersAction {
        public  PageRequest PageRequest { get; }

        public GetUsersAction() {
            PageRequest = new PageRequest( 1, 25 );
        }
    }

    public class GetUsersSuccessAction {
        public  IEnumerable<ShUser>     Users { get; }
        public  PageInformation         PageInformation { get; }

        public GetUsersSuccessAction( IEnumerable<ShUser> userList, PageInformation pageInformation ) {
            Users = userList;
            PageInformation = pageInformation;
        }
    }

    public class GetUsersFailureAction : FailureAction {
        public  GetUsersFailureAction( string message ) :
            base( message ) {
        }
    }
}