using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SlotsGameEngine.BL.BLL.Interfaces;
using SlotsGameEngine.DTO.Client.Requests;
using SlotsGameEngine.DTO.Client.Responses;
using SlotsGameEngine.DTO.Shared.DataObjects;

namespace SlotsGameEngine.BL.BLL
{
    public class GameService : IGameService
    {
        private readonly IAuthService _auth;
        private readonly IRoundService _rounds;
        private readonly IBetService _bets;
        private readonly ITransactionService _transactions;

        public GameService(
            IAuthService auth,
            IRoundService rounds,
            IBetService bets,
            ITransactionService transactions)
        {
            _auth = auth;
            _rounds = rounds;
            _bets = bets;
            _transactions = transactions;
        }

        public async Task<StartResponse> StartAsync(long operatorId, string gameId, StartRequest request, CancellationToken ct)
        {
            // Token is required for real mode (FunMode=0)
            var tokenData = await _auth.ValidateTokenAsync(request.Token ?? string.Empty, ct);
            var sessionId = await _rounds.GenerateRoundIdAsync(); // reusing round-id generator as a simple session id
            var balanceResult = await _transactions.GetBalanceAsync(tokenData, ct);

            var res = new StartResponse
            {
                // BaseResponse defaults: StatusCode=6000, Message="OK"
                Player = new PlayerInfo
                {
                    SessionId = sessionId,
                    Id = tokenData.PlayerId.ToString(),
                    Balance = balanceResult.Balance
                },
                Client = new ClientInfo
                {
                    Type = string.IsNullOrWhiteSpace(request.Client) ? "desktop" : request.Client,
                    Ip = "0.0.0.0",
                    Country = new CountryInfo { Code = "ZA", Name = "South Africa" }
                },
                Currency = new CurrencyInfo
                {
                    IsoCode = request.CurrencyId ?? tokenData.Currency,
                    Name = request.CurrencyId ?? tokenData.Currency,
                    Symbol = (request.CurrencyId ?? tokenData.Currency) == "ZAR" ? "R" : "$",
                    Decimals = 2,
                    DecimalSeparator = ".",
                    ThousandSeparator = ""
                },
                Game = new GameInfo
                {
                    Rtp = 96.0m,
                    Mode = request.FunMode == 1 ? "demo" : "real",
                    FunMode = request.FunMode == 1,
                    MaxWinCap = 0m,
                    Bet = new BetConfig
                    {
                        Default = 1m,
                        Levels = new List<decimal> { 1m, 2m, 5m }
                    },
                    Config = new GameConfig
                    {
                        StartScreen = "main",
                        Settings = { { "autoplay", true }, { "quickSpin", true } }
                    },
                    FreeSpins = null,
                    PromoFreeSpins = null,
                    Feature = new FeatureInfo { Name = "none", Type = "none" },
                    LastPlay = null
                }
            };

            return res;
        }

        public async Task<PlayResponse> PlayAsync(long operatorId, string gameId, PlayRequest request, CancellationToken ct)
        {
            // Null-safe inputs
            var safeBets = request.Bets ?? new List<BetLine>();
            var safeSessionId = request.SessionId ?? string.Empty;

            // We'll treat total bet as sum of all bet lines.
            var totalBet = safeBets.Sum(b => b.Amount);

            // Token for wallet movement would normally come from a session store; here we accept a placeholder
            var tokenData = await _auth.ValidateTokenAsync(string.Empty, ct);

            // ✅ 1) Read balance BEFORE doing anything
            var walletBeforeBet = await _transactions.GetBalanceAsync(tokenData, ct);
            var prevBalance = walletBeforeBet.Balance;

            // Optional: if bet is zero/negative, you can just return OK with same balance
            // (or throw validation error depending on your spec)
            if (totalBet <= 0m)
            {
                return new PlayResponse
                {
                    Player = new PlayerRound
                    {
                        SessionId = safeSessionId,
                        RoundId = string.Empty,
                        Transaction = new TransactionBreakdown { Withdraw = 0m, Deposit = 0m },
                        PrevBalance = prevBalance,
                        Balance = prevBalance,
                        Bet = totalBet,
                        Win = 0m,
                        CurrencyId = tokenData.Currency
                    },
                    Game = new GameRound
                    {
                        Results = null,
                        Mode = "real",
                        MaxWinCap = new MaxWinCap { Achieved = false, Value = 0m, RealWin = 0m }
                    },
                    Jackpots = new List<Jackpot>(),
                    Feature = new FeatureInfo { Name = "none", Type = "none" }
                };
            }

            // ✅ 2) INSUFFICIENT FUNDS guard (prevents negative balance)
            // If your rules allow BonusBalance too, then use it here instead of Balance-only.
            if (totalBet > prevBalance)
            {
                // NOTE: Your DTOs don’t currently define an insufficient funds code.
                // Add a constant later; for now we use a new status code.
                return new PlayResponse
                {
                    StatusCode = 7001,
                    Message = "INSUFFICIENT_FUNDS",
                    Player = new PlayerRound
                    {
                        SessionId = safeSessionId,
                        RoundId = string.Empty,
                        Transaction = new TransactionBreakdown { Withdraw = 0m, Deposit = 0m },
                        PrevBalance = prevBalance,
                        Balance = prevBalance,
                        Bet = totalBet,
                        Win = 0m,
                        CurrencyId = tokenData.Currency
                    },
                    Game = new GameRound
                    {
                        Results = null,
                        Mode = "real",
                        MaxWinCap = new MaxWinCap { Achieved = false, Value = 0m, RealWin = 0m }
                    },
                    Jackpots = new List<Jackpot>(),
                    Feature = new FeatureInfo { Name = "none", Type = "none" }
                };
            }

            // ✅ 3) Withdraw bet only AFTER funds check
            var walletAfterBet = await _transactions.ApplyBetAsync(tokenData, totalBet, ct);

            // ✅ Wallet layer safety-net (in case wallet rules differ / concurrency etc.)
            if (!walletAfterBet.Success)
            {
                return new PlayResponse
                {
                    StatusCode = 7001,
                    Message = "INSUFFICIENT_FUNDS",
                    Player = new PlayerRound
                    {
                        SessionId = safeSessionId,
                        RoundId = string.Empty,
                        Transaction = new TransactionBreakdown { Withdraw = 0m, Deposit = 0m },
                        PrevBalance = prevBalance,
                        Balance = prevBalance,
                        Bet = totalBet,
                        Win = 0m,
                        CurrencyId = tokenData.Currency
                    },
                    Game = new GameRound
                    {
                        Results = null,
                        Mode = "real",
                        MaxWinCap = new MaxWinCap { Achieved = false, Value = 0m, RealWin = 0m }
                    },
                    Jackpots = new List<Jackpot>(),
                    Feature = new FeatureInfo { Name = "none", Type = "none" }
                };
            }

            var roundId = await _rounds.GenerateRoundIdAsync();

            BetResult betResult;
            try
            {
                // ✅ 4) Execute bet/spin
                betResult = await _bets.ExecuteBetAsync(
                    operatorId,
                    gameId,
                    tokenData,
                    new PlayRequest
                    {
                        Bets = safeBets,
                        SessionId = safeSessionId,
                        UserPayload = request.UserPayload
                    },
                    roundId,
                    ct);
            }
            catch
            {
                // ✅ Roll back / refund bet if engine call fails after wallet withdraw
                // You need to implement RefundBetAsync in your transaction service
                await _transactions.RefundBetAsync(tokenData, totalBet, ct);
                throw;
            }

            // ✅ 5) Final balance = prevBalance - bet + win
            var finalBalance = prevBalance - totalBet + betResult.WinAmount;


            var resp = new PlayResponse
            {
                Player = new PlayerRound
                {
                    SessionId = safeSessionId,
                    RoundId = roundId,
                    Transaction = new TransactionBreakdown
                    {
                        Withdraw = totalBet,
                        Deposit = betResult.WinAmount
                    },
                    PrevBalance = prevBalance,
                    Balance = finalBalance,
                    Bet = totalBet,
                    Win = betResult.WinAmount,
                    CurrencyId = tokenData.Currency
                },
                Game = new GameRound
                {
                    Results = betResult.Payload,
                    Mode = "real",
                    MaxWinCap = new MaxWinCap
                    {
                        Achieved = false,
                        Value = 0m,
                        RealWin = betResult.WinAmount
                    }
                },
                Jackpots = new List<Jackpot>(),
                Feature = new FeatureInfo { Name = "none", Type = "none" }
            };

            return resp;
        }


        public async Task<BalanceResponse> GetBalanceAsync(long operatorId, BalanceRequest request, CancellationToken ct)
        {
            // Here the doc prefers playerId; in our sample we reconstruct token via ValidateTokenAsync (adapt as needed)
            var tokenData = await _auth.ValidateTokenAsync(string.Empty, ct);
            var balanceResult = await _transactions.GetBalanceAsync(tokenData, ct);

            return new BalanceResponse
            {
                // BaseResponse defaults OK
                Balance = balanceResult.Balance
            };
        }
    }
}
