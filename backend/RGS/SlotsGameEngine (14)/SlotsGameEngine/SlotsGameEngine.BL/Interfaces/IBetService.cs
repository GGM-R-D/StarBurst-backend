using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SlotsGameEngine.DTO.Client.Requests;
using SlotsGameEngine.DTO.Shared.DataObjects;

namespace SlotsGameEngine.BL.BLL.Interfaces
{
    public interface IBetService
    {
        Task<BetResult> ExecuteBetAsync(
            long operatorId,
            string gameId,
            TokenData token,
            PlayRequest request,
            string roundId,
            CancellationToken ct);
    }
}