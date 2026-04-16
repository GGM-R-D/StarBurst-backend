using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using SlotsGameEngine.BL.BLL.Interfaces;
using SlotsGameEngine.BL.Helpers;              // RngNumberGenerator
using SlotsGameEngine.DTO.Rng.Requests;
using SlotsGameEngine.DTO.Rng.Responses;

namespace SlotsGameEngine.BL.BLL
{
    public class RngService : IRngService
    {
        private readonly RngNumberGenerator _rng;   // local PRNG
        private readonly ILoggingService _logging;

        public RngService(RngNumberGenerator rng, ILoggingService logging)
        {
            _rng = rng;
            _logging = logging;
        }

        public Task<RngResponse> GetRandomAsync(RngRequest req, CancellationToken ct)
        {
            int value;
            if (req.Min.HasValue && req.Max.HasValue)
            {
                var min = req.Min.Value;
                var max = req.Max.Value;
                if (min > max) throw new ArgumentException("min must be <= max");
                value = _rng.Next(min, max);
            }
            else
            {
                // Safe default range
                value = _rng.Next(0, int.MaxValue);
            }

            // Jurisdiction audit (optional)
            if (!string.IsNullOrWhiteSpace(req.GameId) || !string.IsNullOrWhiteSpace(req.RoundId))
            {
                // Serialize request/response bodies to strings; pass null for error; ct last
                var reqJson = JsonSerializer.Serialize(new { req.Min, req.Max, req.GameId, req.RoundId });
                var respJson = JsonSerializer.Serialize(new { random = value });
                _ = _logging.LogApiCallAsync("/rng", "GET", reqJson, respJson, 200, null, ct);
            }

            return Task.FromResult(new RngResponse { Random = value });
        }

        public Task<PoolsResponse> GeneratePoolsAsync(PoolsRequest req, CancellationToken ct)
        {
            if (req.PoolsMap is { Count: > 0 })
            {
                var result = new Dictionary<string, List<int>>();
                foreach (var (name, spec) in req.PoolsMap)
                {
                    var s = spec ?? new PoolSpec();
                    result[name] = GeneratePool(s.Min ?? 0, s.Max ?? int.MaxValue, s.Size ?? 1);
                }

                if (!string.IsNullOrWhiteSpace(req.GameId) || !string.IsNullOrWhiteSpace(req.RoundId))
                {
                    var reqJson = JsonSerializer.Serialize(new { type = "map", req.GameId, req.RoundId });
                    var respJson = JsonSerializer.Serialize(new { pools = result });
                    _ = _logging.LogApiCallAsync("/pools", "POST", reqJson, respJson, 200, null, ct);
                }

                return Task.FromResult(new PoolsResponse { PoolsMap = result });
            }

            var pools = new List<List<int>>();
            foreach (var spec in req.Pools ?? new List<PoolSpec>())
            {
                var min = spec?.Min ?? 0;
                var max = spec?.Max ?? int.MaxValue;
                var size = spec?.Size ?? 1;
                pools.Add(GeneratePool(min, max, size));
            }

            if (!string.IsNullOrWhiteSpace(req.GameId) || !string.IsNullOrWhiteSpace(req.RoundId))
            {
                var reqJson = JsonSerializer.Serialize(new { type = "array", req.GameId, req.RoundId });
                var respJson = JsonSerializer.Serialize(new { pools });
                _ = _logging.LogApiCallAsync("/pools", "POST", reqJson, respJson, 200, null, ct);
            }

            return Task.FromResult(new PoolsResponse { Pools = pools });
        }

        private List<int> GeneratePool(int min, int max, int size)
        {
            if (min > max) throw new ArgumentException("min must be <= max");
            var list = new List<int>(size);
            for (int i = 0; i < size; i++)
                list.Add(_rng.Next(min, max));
            return list;
        }
    }
}
