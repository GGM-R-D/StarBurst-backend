using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SlotsGameEngine.BL.DAL;
using SlotsGameEngine.DTO.Engine.Requests;
using SlotsGameEngine.DTO.Engine.Responses;

namespace SlotsGameEngine.API.Controllers
{
    [ApiController]
    [Route("engine")]  // final URL: POST /engine/play
    public class EngineController : ControllerBase
    {
        private readonly GameEngineClient _engineClient;

        public EngineController(GameEngineClient engineClient)
        {
            _engineClient = engineClient;
        }

        [HttpPost("play")]
        public async Task<ActionResult<EnginePlayResponse>> Play(
            [FromBody] EnginePlayRequest req,
            CancellationToken ct)
        {
            var resp = await _engineClient.PlayAsync(req, ct);
            return Ok(resp);
        }
    }
}
