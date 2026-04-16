using System.Threading;
using System.Threading.Tasks;
using SlotsGameEngine.DTO.Shared.DataObjects;

namespace SlotsGameEngine.BL.BLL.Interfaces
{
    public interface ITransactionService
    {
        Task<WalletTransactionResult> GetBalanceAsync(TokenData tokenData, CancellationToken ct);
        Task<WalletTransactionResult> ApplyBetAsync(TokenData tokenData, decimal amount, CancellationToken ct);

        // ✅ Added for rollback when engine fails after withdrawal
        Task<WalletTransactionResult> RefundBetAsync(TokenData tokenData, decimal amount, CancellationToken ct);
    }
}