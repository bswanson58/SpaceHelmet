using System.Text.Json.Serialization;
using FluentValidation;
using FluentValidation.Results;
using SpaceHelmet.Shared.Constants;

namespace SpaceHelmet.Shared.Dto.Auth {
    public class ChangePasswordRequest {
        public  string      CurrentPassword { get; set; }
        public  string      Password { get; set; }
        public  string      ConfirmPassword { get; set; }

        public const string Route = $"{Routes.BaseRoute}/changePassword";

        [JsonConstructor]
        public ChangePasswordRequest() {
            CurrentPassword = String.Empty;
            Password = String.Empty;
            ConfirmPassword = String.Empty;
        }
    }

    public class ChangePasswordResponse : BaseResponse {
        [JsonConstructor]
        public ChangePasswordResponse( bool succeeded, string message ) :
            base( succeeded, message ) {
        }

        public ChangePasswordResponse() { }

        public ChangePasswordResponse( Exception ex ) :
            base( ex ) {
        }

        public ChangePasswordResponse( ValidationResult validationResult ) :
            base( validationResult ) {
        }

        public ChangePasswordResponse( IEnumerable<string> errors ) :
            base( false, string.Join( Environment.NewLine, errors )) {
        }
    }

    // ReSharper disable once UnusedType.Global
    public class ChangePasswordRequestValidator : AbstractValidator<ChangePasswordRequest> {
        public ChangePasswordRequestValidator() {
            RuleFor( p => p.CurrentPassword )
                .NotEmpty()
                .WithMessage( "Current password must be specified" );

            RuleFor( p => p.CurrentPassword )
                .MaximumLength( 32 )
                .WithMessage( "Current password is too long" );

            RuleFor( p => p.Password )
                .NotEmpty()
                .WithMessage( "Password must be specified" );

            RuleFor( p => p.Password )
                .MaximumLength( 32 )
                .WithMessage( "New password is too long" );

            RuleFor( p => p.ConfirmPassword )
                .NotEmpty()
                .WithMessage( "Confirmation of password must be specified" );

            RuleFor( p => p.ConfirmPassword )
                .MaximumLength( 32 )
                .WithMessage( "New password is too long" );

            RuleFor( p => p.Password )
                .Matches( p => p.ConfirmPassword )
                .WithMessage( "Password entries must match" );
        }
    }
}
