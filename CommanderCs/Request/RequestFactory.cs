using System.Threading;

namespace Commander
{
    class RequestFactory
    {
        public int Number { get => number; }

        public Request Create() => new Request(this, Interlocked.Increment(ref number)); 

        public int number;
    }
}
