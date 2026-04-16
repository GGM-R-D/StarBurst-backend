using System.Collections.Generic;

namespace SlotsGameEngine.DTO.Rng.Responses
{
    // Return whichever matches input style
    public class PoolsResponse
    {
        public List<List<int>>? Pools { get; set; }
        public Dictionary<string, List<int>>? PoolsMap { get; set; }
    }
}
