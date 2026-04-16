using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SlotsGameEngine.BL.BLL.Interfaces;
using SlotsGameEngine.DTO.Rng.Requests;
using SlotsGameEngine.DTO.Rng.Responses;

namespace SlotsGameEngine.API.Controllers
{
    [ApiController]
    [Route("rng")]
    public class RngController : ControllerBase
    {
        private readonly IRngService _rng;

        public RngController(IRngService rng) => _rng = rng;

        [HttpGet]
        public async Task<ActionResult<RngResponse>> Get(
    [FromQuery] int? min,
    [FromQuery] int? max,
    [FromQuery] string? gameId,
    [FromQuery] string? roundId,
    CancellationToken ct)
        {
            if (min.HasValue != max.HasValue)
                return BadRequest(new { message = "Provide both min and max, or neither." });

            if (min.HasValue && max.HasValue && min > max)
                return BadRequest(new { message = "min must be <= max." });

            var resp = await _rng.GetRandomAsync(new RngRequest { Min = min, Max = max, GameId = gameId, RoundId = roundId }, ct);
            return Ok(resp);
        }
    }
}
