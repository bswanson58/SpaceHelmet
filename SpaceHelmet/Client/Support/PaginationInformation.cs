namespace SpaceHelmet.Client.Support {
    public class PaginationInformation {
        public  int     PageCount { get; }
        public  bool    NoIssues { get; }
        public  bool    DisplayPageControl { get; }

        public PaginationInformation() {
            PageCount = 1;
            DisplayPageControl = false;
        }

        public PaginationInformation( int pageCount, bool displayPageControl, bool noIssues ) {
            PageCount = pageCount;
            DisplayPageControl = displayPageControl;
            NoIssues = noIssues;
        }
    }
}
