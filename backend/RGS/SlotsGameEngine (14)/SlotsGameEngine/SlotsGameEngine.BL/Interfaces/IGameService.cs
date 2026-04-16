using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using SlotsGameEngine.DTO.Client.Requests;
using SlotsGameEngine.DTO.Client.Responses;

namespace SlotsGameEngine.BL.BLL.Interfaces
{
    public interface IGameService
    {
        Task<StartResponse> StartAsync(long operatorId, string gameId, StartRequest request, CancellationToken ct);
        Task<PlayResponse> PlayAsync(long operatorId, string gameId, PlayRequest request, CancellationToken ct);
        Task<BalanceResponse> GetBalanceAsync(long operatorId, BalanceRequest request, CancellationToken ct);
    }
}
