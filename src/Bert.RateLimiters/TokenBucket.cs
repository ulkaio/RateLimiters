using System;

namespace Bert.RateLimiters
{
    public abstract class TokenBucket : IThrottleStrategy
    {
        private static readonly object syncRoot = new object();

        // Max token capacity of a bucket
        protected long bucketTokenCapacity;

        // No of ticks per refill interval, let's say this value is 5, so we will be wait 5ticks for the next refill. 
        protected readonly long ticksPerRefillInterval;

        // UTC time for the next refill
        protected long nextRefillTimeInTicks;

        // Number of tokens in the bucket
        protected long tokens;

        protected TokenBucket(long bucketTokenCapacity, long refillInterval, long refillIntervalInMilliSeconds)
        {
            if (bucketTokenCapacity <= 0)
                throw new ArgumentOutOfRangeException(nameof(bucketTokenCapacity),
                    "bucket token capacity can not be negative");
            if (refillInterval < 0)
                throw new ArgumentOutOfRangeException(nameof(refillInterval), "Refill interval cannot be negative");
            if (refillIntervalInMilliSeconds <= 0)
                throw new ArgumentOutOfRangeException(nameof(refillIntervalInMilliSeconds),
                    "Refill interval in milliseconds cannot be negative");

            this.bucketTokenCapacity = bucketTokenCapacity;
            ticksPerRefillInterval = TimeSpan.FromMilliseconds(refillInterval * refillIntervalInMilliSeconds).Ticks;
        }

        public bool ShouldThrottle(long charge = 1)
        {
            return ShouldThrottle(charge, out _);
        }


        public bool ShouldThrottle(long charge, out TimeSpan waitTime)
        {
            if (charge <= 0)
                throw new ArgumentOutOfRangeException(nameof(charge), "Should be positive integer");

            lock (syncRoot)
            {
                UpdateTokensInBucket();

                // If there isn't enough token, request need to throttle
                if (tokens < charge)
                {
                    var timeToIntervalEnd = nextRefillTimeInTicks - SystemTime.UtcNow.Ticks;
                    if (timeToIntervalEnd < 0) return ShouldThrottle(charge, out waitTime);

                    waitTime = TimeSpan.FromTicks(timeToIntervalEnd);
                    return true;
                }

                tokens -= charge;
                waitTime = TimeSpan.Zero;
                return false;
            }
        }

        protected abstract void UpdateTokensInBucket();

        public bool ShouldThrottle(out TimeSpan waitTime)
        {
            return ShouldThrottle(1, out waitTime);
        }

        public long CurrentTokenCount
        {
            get
            {
                lock (syncRoot)
                {
                    UpdateTokensInBucket();
                    return tokens;
                }
            }
        }
    }
}