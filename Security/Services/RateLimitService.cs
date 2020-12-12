

namespace RateLimiting.Security.Services
{
    public interface IRateLimitService
    {
        int processRequest();
    }

    public class RateLimitService : IRateLimitService
    {
        private int _limit = 1024;

        public RateLimitService() {}

        public int processRequest() {
            return _limit;
        }

        private int useBandwidth(int bytes) {
            if (_limit > bytes) {
                _limit -= bytes;
                return _limit;
            };
            return -1;
        }

        private int resetBandwidth() {
            _limit = 1024;
            return _limit;
        }

    }
}