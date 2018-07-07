using System;

namespace Bert.RateLimiters
{
    public class Throttler
    {
        private readonly IThrottleStrategy strategy;

        public Throttler(IThrottleStrategy strategy)
        {
            this.strategy = strategy ?? throw new ArgumentNullException(nameof(strategy));
        }

        public bool CanConsume()
        {
            return !strategy.ShouldThrottle();
        }
    }
}