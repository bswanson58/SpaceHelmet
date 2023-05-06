namespace SpaceHelmet.Client.Ui.Actions {
    public class ApiCallStarted {
        public string   AnnouncementMessage { get; }

        public ApiCallStarted( string message ) {
            AnnouncementMessage = message;
        }
    }

    public class ApiCallCompleted { }

    public class ApiCallFailure {
        public  string  Message { get; }

        public ApiCallFailure( string message ) {
            Message = message;
        }
    }
}
