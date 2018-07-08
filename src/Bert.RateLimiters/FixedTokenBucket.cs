namespace Bert.RateLimiters
{
    public class FixedTokenBucket : TokenBucket
    {
        public FixedTokenBucket(
            long bucketTokenCapacity,
            long refillInterval,
            long refillIntervalInMilliSeconds) : base(bucketTokenCapacity, refillInterval, refillIntervalInMilliSeconds)
        {
        }

        protected override void UpdateTokensInBucket()
        {
            var currentTimeInTicks = SystemTime.UtcNow.Ticks;
            if (currentTimeInTicks < nextRefillTimeInTicks) return;

            // Full refill after interval
            tokens = bucketTokenCapacity;
            nextRefillTimeInTicks = currentTimeInTicks + ticksPerRefillInterval;
        }
    }
}