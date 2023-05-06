using SpaceHelmet.Shared.Dto;

namespace SpaceHelmet.Server.Support {
    public class PagedList<T> : List<T> {
        public  PageInformation     PageInformation { get; }

        private PagedList( IEnumerable<T> items, int count, PageRequest pageRequest ) {
            PageInformation = new PageInformation( pageRequest, count );

            AddRange( items );
        }

        public static PagedList<T> CreatePagedList( IQueryable<T> source, PageRequest pageRequest ) {
            var count = source.Count();
            var items = source
                .Skip(( pageRequest.PageNumber - 1 ) * pageRequest.MaxPageSize )
                .Take( pageRequest.MaxPageSize );

            return new PagedList<T>( items, count, pageRequest );
        }

        public static PagedList<T> CreatePagedList( IList<T> source, PageRequest pageRequest ) {
            var count = source.Count();
            var items = source
                .Skip(( pageRequest.PageNumber - 1 ) * pageRequest.MaxPageSize )
                .Take( pageRequest.MaxPageSize );

            return new PagedList<T>( items, count, pageRequest );
        }

        public static PagedList<T> CreatePagedList( IEnumerable<T> source, PageRequest pageRequest ) {
            var count = source.Count();
            var items = source
                .Skip(( pageRequest.PageNumber - 1 ) * pageRequest.MaxPageSize )
                .Take( pageRequest.MaxPageSize );

            return new PagedList<T>( items, count, pageRequest );
        }
    }
}
