using System.Collections.Concurrent;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using SlotsGameEngine.DTO.Shared.DataObjects;

namespace SlotsGameEngine.BL.DAL
{
    public class OperatorWalletClient
    {
        private readonly HttpClient _httpClient;

        // In-memory stub balances per player (until real wallet integration)
        private static readonly ConcurrentDictionary<long, decimal> _balances = new();

        private const decimal DefaultStubBalance = 1000m;

        public OperatorWalletClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        private static decimal GetOrInitBalance(long playerId)
            => _balances.GetOrAdd(playerId, DefaultStubBalance);

        public async Task<WalletTransactionResult> GetBalanceAsync(TokenData token, CancellationToken ct)
        {
            await Task.Yield(); // TODO: call real wallet

            var bal = GetOrInitBalance(token.PlayerId);

            return new WalletTransactionResult
            {
                Success = true,
                Balance = bal,
                Currency = token.Currency
            };
        }

        public async Task<WalletTransactionResult> ApplyBetAsync(TokenData token, decimal amount, CancellationToken ct)
        {
            await Task.Yield(); // TODO: call real wallet

            if (amount <= 0m)
            {
                var bal0 = GetOrInitBalance(token.PlayerId);
                return new WalletTransactionResult
                {
                    Success = true,
                    Balance = bal0,
                    Currency = token.Currency
                };
            }

            while (true)
            {
                var current = GetOrInitBalance(token.PlayerId);

                // ✅ Prevent negative balances
                if (amount > current)
                {
                    return new WalletTransactionResult
                    {
                        Success = false,
                        Balance = current,
                        Currency = token.Currency
                    };
                }

                var next = current - amount;

                // atomic update
                if (_balances.TryUpdate(token.PlayerId, next, current))
                {
                    return new WalletTransactionResult
                    {
                        Success = true,
                        Balance = next,
                        Currency = token.Currency
                    };
                }
            }
        }

        public async Task<WalletTransactionResult> RefundBetAsync(TokenData token, decimal amount, CancellationToken ct)
        {
            await Task.Yield(); // TODO: call real wallet

            if (amount <= 0m)
            {
                var bal0 = GetOrInitBalance(token.PlayerId);
                return new WalletTransactionResult
                {
                    Success = true,
                    Balance = bal0,
                    Currency = token.Currency
                };
            }

            while (true)
            {
                var current = GetOrInitBalance(token.PlayerId);
                var next = current + amount;

                if (_balances.TryUpdate(token.PlayerId, next, current))
                {
                    return new WalletTransactionResult
                    {
                        Success = true,
                        Balance = next,
                        Currency = token.Currency
                    };
                }
            }
        }
    }
}
