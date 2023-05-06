using FluentValidation;
using System.Text.Json.Serialization;

namespace SpaceHelmet.Shared.Dto {
    public class PageRequest {
        public  int     PageNumber { get; }
        public  int     MaxPageSize { get; }

        [JsonConstructor]
        public PageRequest( int pageNumber, int maxPageSize ) {
            PageNumber = pageNumber;
            MaxPageSize = maxPageSize;
        }
    }

    public class PageInformation {
        public  int     CurrentPage { get; }
        public  int     TotalPages { get; }
        public  int     PageSize { get; }
        public  int     TotalCount { get; }

        public  bool    HasPrevious => CurrentPage > 1;
        public  bool    HasNext => CurrentPage < TotalPages;

        [JsonConstructor]
        public PageInformation( int currentPage, int totalPages, int pageSize, int totalCount ) {
            CurrentPage = currentPage;
            TotalPages = totalPages;
            PageSize = pageSize;
            TotalCount = totalCount;
        }

        public PageInformation( PageRequest pageRequest, int totalCount ) {
            CurrentPage = pageRequest.PageNumber;
            PageSize = pageRequest.MaxPageSize;
            TotalCount = totalCount;

            TotalPages = (int)Math.Ceiling( TotalCount / (double)pageRequest.MaxPageSize );
        }

        public static PageInformation Default =>
            new ( 0, 0, 0, 0 );

        [JsonIgnore]
        public PageInformation ReduceTotal =>
            new ( CurrentPage, TotalPages, PageSize, TotalCount - 1 );

        [JsonIgnore]
        public PageInformation IncreaseTotal =>
            new ( CurrentPage, TotalPages, PageSize, TotalCount + 1 );
    }

    public class PageRequestValidator : AbstractValidator<PageRequest> {
        public PageRequestValidator() {
            RuleFor( p => p.PageNumber )
                .GreaterThanOrEqualTo( 1 )
                .WithMessage( "Requested page number must be greater than zero." );

            RuleFor( p => p.MaxPageSize )
                .GreaterThanOrEqualTo( 1 )
                .WithMessage( "Requested page size must be greater than zero." );

            RuleFor( p => p.MaxPageSize )
                .LessThanOrEqualTo( 100 )
                .WithMessage( "Requested page size must be 100 or less." );
        }
    }
}
