using Microsoft.AspNetCore.Mvc;
using SlotsGameEngine.BL.DAL;
using SlotsGameEngine.DTO.Engine.Requests;
using SlotsGameEngine.DTO.Engine.Responses;

namespace SlotsGameEngine.API.Controllers
{
    [ApiController]
    [Route("")]
    public class RootEngineController : ControllerBase
    {
        private readonly GameEngineClient _engineClient;

        public RootEngineController(GameEngineClient engineClient)
        {
            _engineClient = engineClient;
        }

        // This will handle POST /play
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
