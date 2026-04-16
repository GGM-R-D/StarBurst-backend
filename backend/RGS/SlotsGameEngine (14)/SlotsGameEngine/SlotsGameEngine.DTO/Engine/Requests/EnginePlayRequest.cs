using System.Collections.Generic;

namespace SlotsGameEngine.DTO.Engine.Requests
{
    public class EnginePlayRequest
    {
        // ⭐ Required by starburst engine
        public string? GameId { get; set; }
        public string? PlayerToken { get; set; }

        // Forwarded from client
        public string? SessionId { get; set; }

        // bets: [{ amount, betType }]
        public List<BetLine> Bets { get; set; } = new();

        // total bet = sum(bets.amount)
        public decimal Bet { get; set; }

        public decimal TotalBet { get; set; }

        public object? UserPayload { get; set; }
        public object? LastResponse { get; set; }

        // optional RTP level selector from the math doc
        public int? RtpLevel { get; set; }

        // 0 normal, 1 in-game free spins, 2 bonus, 3 free bets (per doc)
        public int Mode { get; set; } = 0;

        public Currency Currency { get; set; } = new Currency();
    }

    public class BetLine
    {
        public decimal Amount { get; set; }

        // ⭐ Required by starburst engine: Bets[0].BetType
        public string? BetType { get; set; }
    }

    public class Currency
    {
        // default – will be overridden by token.Currency
        public string Id { get; set; } = "USD";
    }
}