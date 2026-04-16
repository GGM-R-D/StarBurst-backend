using Microsoft.AspNetCore.Mvc;
using SlotsGameEngine.BL.BLL.Interfaces;
using SlotsGameEngine.DTO.Client.Requests;
using SlotsGameEngine.DTO.Client.Responses;
using System.Threading;
using System.Threading.Tasks;

namespace SlotsGameEngine.API.Controllers
{
    [ApiController]
    [Route("{operatorId:long}/{gameId}")]
    public class GameController : ControllerBase
    {
        private readonly IGameService _game;

        public GameController(IGameService game) => _game = game;

        [HttpPost("start")]
        public async Task<ActionResult<StartResponse>> Start(
            long operatorId,
            string gameId,
            [FromBody] StartRequest request,
            CancellationToken ct)
        {
            var resp = await _game.StartAsync(operatorId, gameId, request, ct);
            return Ok(resp);
        }

        [HttpPost("play")]
        public async Task<ActionResult<PlayResponse>> Play(
            long operatorId,
            string gameId,
            [FromBody] PlayRequest request,
            CancellationToken ct)
        {
            var resp = await _game.PlayAsync(operatorId, gameId, request, ct);
            return Ok(resp);
        }
    }
}
