using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SlotsGameEngine.BL.BLL.Interfaces;
using SlotsGameEngine.DTO.Rng.Requests;
using SlotsGameEngine.DTO.Rng.Responses;

namespace SlotsGameEngine.API.Controllers
{
    [ApiController]
    [Route("pools")]
    public class PoolsController : ControllerBase
    {
        private readonly IRngService _rng;

        public PoolsController(IRngService rng) => _rng = rng;

        [HttpPost]
        public async Task<ActionResult<PoolsResponse>> Generate([FromBody] PoolsRequest request, CancellationToken ct)
        {
            // Normalize to internal model with defaults
            if ((request.Pools == null || request.Pools.Count == 0) && (request.PoolsMap == null || request.PoolsMap.Count == 0))
                return BadRequest(new { message = "Provide either 'pools' array or 'pools' map." });

            var resp = await _rng.GeneratePoolsAsync(request, ct);
            return Ok(resp);
        }

    }
}
