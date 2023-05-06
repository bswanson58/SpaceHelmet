namespace SpaceHelmet.Client.Store {
    public abstract class RootState {
        public  bool                ApiCallInProgress { get; }
        public  string              ApiCallMessage { get; }

        public RootState( bool callInProgress, string callMessage ) {
            ApiCallInProgress = callInProgress;
            ApiCallMessage = callMessage;
        }
    }
}
