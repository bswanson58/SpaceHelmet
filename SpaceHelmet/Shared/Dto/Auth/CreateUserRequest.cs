using System.Text.Json.Serialization;
using FluentValidation;
using FluentValidation.Results;
using SpaceHelmet.Shared.Constants;

namespace SpaceHelmet.Shared.Dto.Auth {
    public class CreateUserRequest {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }

        public const string Route = $"{Routes.BaseRoute}/createUser";

        public CreateUserRequest() {
            Name = string.Empty;
            Email = string.Empty;
            Password = string.Empty;
            ConfirmPassword = string.Empty;
        }
    }

    public class CreateUserResponse : BaseResponse {
        [JsonConstructor]
        public CreateUserResponse( bool succeeded, string message ) :
            base( succeeded, message ) {
        }

        public CreateUserResponse() { }

        public CreateUserResponse( Exception ex ) :
            base( ex ) {
        }

        public CreateUserResponse( ValidationResult validationResult ) :
            base( validationResult ) {
        }

        public CreateUserResponse( IEnumerable<string> errors ) :
            base( false, string.Join( Environment.NewLine, errors ) ) {
        }
    }

    public class CreateUserRequestValidator : AbstractValidator<CreateUserRequest> {
        public CreateUserRequestValidator() {
            RuleFor( p => p.Name ).NotEmpty().WithMessage( "User name is required." );
            RuleFor( p => p.Email ).NotEmpty().WithMessage( "User email is required." );
            RuleFor( p => p.Email ).EmailAddress().WithMessage( "Email entry is not a valid email address" );
            RuleFor( p => p.Password ).MinimumLength( 6 ).WithMessage( "Password must be at least 6 characters" );
            RuleFor( p => p.ConfirmPassword ).Matches( p => p.Password ).WithMessage( "Password entries must match" );
        }
    }
}
