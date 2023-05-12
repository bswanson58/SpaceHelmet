using System;

namespace TokenClientSupport.Support {
    public interface ITimeProvider {
        DateOnly        CurrentDate { get; }
        DateTime        CurrentDateTime { get; }
        DateTime        CurrentUtcTime { get; }
    }

    public static class DateTimeProvider {
        private static ITimeProvider ?  mTimeProvider;

        public  static ITimeProvider    Instance => mTimeProvider ?? CreateProvider();

        private static ITimeProvider CreateProvider() {
            mTimeProvider = new RuntimeTimeProvider();

            return mTimeProvider;
        }

        public static void SetProvider( ITimeProvider provider ) {
            mTimeProvider = provider;
        }
    }

    internal class RuntimeTimeProvider : ITimeProvider {
        public DateOnly CurrentDate => DateOnly.FromDateTime( DateTime.Now );
        public DateTime CurrentDateTime => DateTime.Now;
        public DateTime CurrentUtcTime => DateTime.UtcNow;
    }

    public class TestTimeProvider : ITimeProvider {
        private readonly DateTime   mTime;

        public TestTimeProvider( DateTime forTime ) {
            mTime = forTime;
        }

        public DateOnly CurrentDate => DateOnly.FromDateTime( mTime );
        public DateTime CurrentDateTime => mTime;
        public DateTime CurrentUtcTime => mTime.ToUniversalTime();
    }
}
