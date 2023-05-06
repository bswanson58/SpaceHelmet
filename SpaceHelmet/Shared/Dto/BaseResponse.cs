using FluentValidation.Results;

namespace SpaceHelmet.Shared.Dto {
    public class BaseResponse {
        public  bool    Succeeded { get; }
        public  string  Message { get; }

        protected BaseResponse() {
            Succeeded = true;
            Message = String.Empty;
        }

        protected BaseResponse( Exception ex ) {
            Succeeded = false;
            Message = ex.Message;
        }

        protected BaseResponse( ValidationResult validationResult ) {
            Succeeded = validationResult.IsValid;
            Message = validationResult.ToString();
        }

        protected BaseResponse( bool succeeded, string message ) {
            Succeeded = succeeded;
            Message = message;
        }
    }
}
