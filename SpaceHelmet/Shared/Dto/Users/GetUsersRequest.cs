using FluentValidation;
using SpaceHelmet.Shared.Constants;
using SpaceHelmet.Shared.Entities;
using System.Text.Json.Serialization;
using FluentValidation.Results;

namespace SpaceHelmet.Shared.Dto.Users {
    public class GetUsersRequest {
        public  PageRequest PageRequest { get; }

        public const string Route = $"{Routes.BaseRoute}/getUsers";

        [JsonConstructor]
        public GetUsersRequest( PageRequest pageRequest ) {
            PageRequest = pageRequest;
        }
    }

    public class GetUsersResponse : BaseResponse {
        public  PageInformation     PageInformation { get; }
        public  List<ShUser>        Users { get; }

        [JsonConstructor]
        public GetUsersResponse( bool succeeded, string message, List<ShUser> users, PageInformation pageInformation ) :
            base( succeeded, message ) {
            PageInformation = pageInformation;
            Users = users;
        }

        public GetUsersResponse() {
            Users = new List<ShUser>();
            PageInformation = PageInformation.Default;
        }

        public GetUsersResponse( List<ShUser> users, PageInformation pageInformation ) {
            Users = new List<ShUser>( users );
            PageInformation = pageInformation;
        }

        public GetUsersResponse( ValidationResult validationResult ) :
            base ( validationResult ) {
            Users = new List<ShUser>();
            PageInformation = PageInformation.Default;
        }

        public GetUsersResponse( Exception ex ) :
            base( ex ) {
            Users = new List<ShUser>();
            PageInformation = PageInformation.Default;
        }
    }

    // ReSharper disable once UnusedType.Global
    public class GetUsersRequestValidator : AbstractValidator<GetUsersRequest> {
        public GetUsersRequestValidator() {
            RuleFor( p => p.PageRequest )
                .SetValidator( new PageRequestValidator());
        }
    }
}
