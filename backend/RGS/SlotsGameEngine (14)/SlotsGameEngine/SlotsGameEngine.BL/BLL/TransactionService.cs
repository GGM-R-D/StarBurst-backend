using System.Threading;
using System.Threading.Tasks;
using SlotsGameEngine.BL.BLL.Interfaces;
using SlotsGameEngine.BL.DAL;
using SlotsGameEngine.DTO.Shared.DataObjects;

namespace SlotsGameEngine.BL.BLL
{
    public class TransactionService : ITransactionService
    {
        private readonly OperatorWalletClient _walletClient;

        public TransactionService(OperatorWalletClient walletClient)
        {
            _walletClient = walletClient;
        }

        public Task<WalletTransactionResult> GetBalanceAsync(TokenData tokenData, CancellationToken ct)
        {
            return _walletClient.GetBalanceAsync(tokenData, ct);
        }

        public Task<WalletTransactionResult> ApplyBetAsync(TokenData tokenData, decimal amount, CancellationToken ct)
        {
            return _walletClient.ApplyBetAsync(tokenData, amount, ct);
        }
        public Task<WalletTransactionResult> RefundBetAsync(TokenData tokenData, decimal amount, CancellationToken ct)
        {
            return _walletClient.RefundBetAsync(tokenData, amount, ct);
        }
    }
}
