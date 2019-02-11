namespace Commander
{
    class Request
    {
        public Request(RequestFactory requestFactory, int number)
        {
            this.requestFactory = requestFactory;
            this.number = number;
        }

        public bool IsCancelled => number < requestFactory.Number;

        readonly RequestFactory requestFactory;
        readonly int number;
    }
}
