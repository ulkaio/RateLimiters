namespace Bert.RateLimiters
{
    public class StepDownTokenBucket : LeakyTokenBucket
    {
        public StepDownTokenBucket(
            long bucketTokenCapacity,
            long refillInterval,
            int refillIntervalInMilliSeconds,
            long stepTokens,
            long stepInterval,
            int stepIntervalInMilliseconds) : base(bucketTokenCapacity, refillInterval, refillIntervalInMilliSeconds,
            stepTokens,
            stepInterval, stepIntervalInMilliseconds)
        {
        }

        protected override void UpdateTokensInBucket()
        {
            var currentTimeInTicks = SystemTime.UtcNow.Ticks;

            if (currentTimeInTicks >= nextRefillTimeInTicks)
            {
                // Set tokens to max
                tokens = bucketTokenCapacity;

                // Compute next refill time
                nextRefillTimeInTicks = currentTimeInTicks + ticksPerRefillInterval;
                return;
            }

            // Calculate max tokens possible till the end
            var timeToNextRefill = nextRefillTimeInTicks - currentTimeInTicks;
            var stepsToNextRefill = timeToNextRefill / ticksStepInterval;

            var maxPossibleTokens = stepsToNextRefill * stepTokens;

            if (timeToNextRefill % ticksStepInterval > 0)
                maxPossibleTokens += stepTokens;

            if (maxPossibleTokens < tokens)
                tokens = maxPossibleTokens;
        }
    }
}