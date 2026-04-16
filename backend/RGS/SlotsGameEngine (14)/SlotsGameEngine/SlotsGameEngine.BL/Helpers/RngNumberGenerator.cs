using System;

namespace SlotsGameEngine.BL.Helpers
{
    /// <summary>
    /// Local RNG helper. Next(min,max) returns an integer in [min, max] inclusive.
    /// </summary>
    public class RngNumberGenerator
    {
        public int Next(int minInclusive, int maxInclusive)
        {
            if (minInclusive > maxInclusive)
                throw new ArgumentException("min must be <= max");

            // Random.Next upper bound is exclusive; make ours inclusive.
            long upperExclusive = (long)maxInclusive + 1;
            if (upperExclusive > int.MaxValue) upperExclusive = int.MaxValue;

            return Random.Shared.Next(minInclusive, (int)upperExclusive);
        }
    }
}
