namespace RateLimiting.Security.Services
{
    public interface IRateLimitService
    {
        int processRequest(int bytes);
        int reportBandwidth();
    }

    public class RateLimitService : IRateLimitService
    {
        private int _limit = 1024;
        private int _bandwidth = 1024;

        public RateLimitService() {}

        public int processRequest(int bytes) {
            if (_bandwidth > bytes) {
                _bandwidth -= bytes;
                return _bandwidth;
            };
            return -1;
        }

        public int reportBandwidth()
        {
            return _bandwidth;
        }

        private int resetBandwidth() {
            _bandwidth = _limit;
            return _bandwidth;
        }

    }
}