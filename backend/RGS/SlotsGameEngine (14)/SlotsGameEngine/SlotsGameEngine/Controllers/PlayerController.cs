using Microsoft.AspNetCore.Mvc;
using SlotsGameEngine.BL.BLL.Interfaces;
using SlotsGameEngine.DTO.Client.Requests;
using SlotsGameEngine.DTO.Client.Responses;
using System.Threading;
using System.Threading.Tasks;

namespace SlotsGameEngine.API.Controllers
{
    [ApiController]
    [Route("{operatorId:long}/player")]
    public class PlayerController : ControllerBase
    {
        private readonly IGameService _game;

        public PlayerController(IGameService game) => _game = game;

        [HttpPost("balance")]
        public async Task<ActionResult<BalanceResponse>> Balance(
            long operatorId,
            [FromBody] BalanceRequest request,
            CancellationToken ct)
        {
            var resp = await _game.GetBalanceAsync(operatorId, request, ct);
            return Ok(resp);
        }
    }
}
