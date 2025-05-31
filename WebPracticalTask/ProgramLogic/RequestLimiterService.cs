namespace WebPracticalTask.ProgramLogic
{
    public class RequestLimiterService
    {
        private readonly int _parallelLimit;
        private int _currentRequests;

        public RequestLimiterService(IConfiguration configuration)
        {
            _parallelLimit = configuration.GetValue("Settings:ParallelLimit", 5);
        }

        public bool TryAcquireSlot()
        {
            lock (this)
            {
                if (_currentRequests < _parallelLimit)
                {
                    _currentRequests++;
                    return true;
                }
                return false;
            }
        }

        public void ReleaseSlot()
        {
            lock (this)
            {
                if (_currentRequests > 0)
                    _currentRequests--;
            }
        }

        public (int current, int limit) GetStatus()
        {
            lock (this)
            {
                return (_currentRequests, _parallelLimit);
            }
        }
    }
}
