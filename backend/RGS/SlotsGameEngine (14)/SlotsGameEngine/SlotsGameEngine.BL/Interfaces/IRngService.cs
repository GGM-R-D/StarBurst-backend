using System.Threading;
using System.Threading.Tasks;
using SlotsGameEngine.DTO.Rng.Requests;
using SlotsGameEngine.DTO.Rng.Responses;

namespace SlotsGameEngine.BL.BLL.Interfaces
{
    public interface IRngService
    {
        Task<RngResponse> GetRandomAsync(RngRequest req, CancellationToken ct);
        Task<PoolsResponse> GeneratePoolsAsync(PoolsRequest req, CancellationToken ct);
    }
}
