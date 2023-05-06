namespace SpaceHelmet.Client.Store {
    public class FailureAction {
        public  string  Message { get; }

        protected FailureAction( string message ) {
            Message = message;
        }
    }
}
