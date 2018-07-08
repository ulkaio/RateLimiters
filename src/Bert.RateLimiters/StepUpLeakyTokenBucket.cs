namespace Bert.RateLimiters
{
    public class StepUpLeakyTokenBucket : LeakyTokenBucket
    {
        private long lastActivityTime;

        public StepUpLeakyTokenBucket(
            long bucketTokenCapacity, 
            long refillInterval, 
            int refillIntervalInMilliSeconds,
            long stepTokens, 
            long stepInterval, 
            int stepIntervalInMilliseconds) : base(bucketTokenCapacity, refillInterval,
            refillIntervalInMilliSeconds, stepTokens, stepInterval, stepIntervalInMilliseconds)
        {
        }

        protected override void UpdateTokensInBucket()
        {
            var currentTimeInTicks = SystemTime.UtcNow.Ticks;

            if (currentTimeInTicks >= nextRefillTimeInTicks)
            {
                tokens = stepTokens;
                lastActivityTime = currentTimeInTicks;
                nextRefillTimeInTicks = currentTimeInTicks + ticksPerRefillInterval;
                return;
            }

            // Calculate tokens at current step
            long elapsedTimeSinceLastActivity = currentTimeInTicks - lastActivityTime;
            long elapsedStepsSinceLastActivity = elapsedTimeSinceLastActivity / ticksStepInterval;

            tokens += elapsedStepsSinceLastActivity * stepTokens;

            if (tokens > bucketTokenCapacity) tokens = bucketTokenCapacity;
            lastActivityTime = currentTimeInTicks;
        }
    }
}