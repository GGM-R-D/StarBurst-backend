using System.Collections.Generic;

namespace SlotsGameEngine.DTO.Rng.Requests
{
    // Accept EITHER arrays or named-map; one of Pools or PoolsMap must be provided.
    public class PoolsRequest
    {
        public List<PoolSpec>? Pools { get; set; }                      // array form
        public Dictionary<string, PoolSpec>? PoolsMap { get; set; }     // map form
        public string? GameId { get; set; }                              // jurisdiction (optional)
        public string? RoundId { get; set; }                             // jurisdiction (optional)
    }

    public class PoolSpec
    {
        public int? Min { get; set; }    // default 0
        public int? Max { get; set; }    // default int.MaxValue
        public int? Size { get; set; }   // default 1
    }
}
