using System;

namespace Bert.RateLimiters
{
    public interface IThrottleStrategy
    {
        bool ShouldThrottle(long charge = 1);
        bool ShouldThrottle(long charge, out TimeSpan waitTime);
        bool ShouldThrottle(out TimeSpan waitTime);
        long CurrentTokenCount { get; }
    }
}