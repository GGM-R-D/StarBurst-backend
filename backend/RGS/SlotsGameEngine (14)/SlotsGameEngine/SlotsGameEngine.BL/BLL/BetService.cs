using System.Threading;
using System.Threading.Tasks;
using SlotsGameEngine.BL.BLL.Interfaces;
using SlotsGameEngine.BL.DAL;
using SlotsGameEngine.DTO.Client.Requests;
using SlotsGameEngine.DTO.Shared.DataObjects; // <-- REQUIRED

namespace SlotsGameEngine.BL.BLL
{
    public class BetService : IBetService
    {
        private readonly RgsClient _rgsClient;
        public BetService(RgsClient rgsClient) { _rgsClient = rgsClient; }

        public Task<BetResult> ExecuteBetAsync(
            long operatorId,
            string gameId,
            SlotsGameEngine.DTO.Shared.DataObjects.TokenData token, // <-- ensure this is DTO
            PlayRequest request,
            string roundId,
            CancellationToken ct)
        {
            return _rgsClient.PlayAsync(operatorId, gameId, token, request, roundId, ct);
        }
    }
}