using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections.Generic;

namespace SlotsGameEngine.DTO.Shared.DataObjects
{
    // --- Common leaf objects used by Start/Play ---

    public class PlayerInfo
    {
        public string SessionId { get; set; } = string.Empty;
        public string Id { get; set; } = string.Empty;       // playerId
        public decimal Balance { get; set; }
    }

    public class ClientInfo
    {
        public string Type { get; set; } = "desktop";        // desktop|mobile
        public string Ip { get; set; } = "0.0.0.0";
        public CountryInfo Country { get; set; } = new();
    }

    public class CountryInfo
    {
        public string Code { get; set; } = "ZA";
        public string Name { get; set; } = "South Africa";
    }

    public class CurrencyInfo
    {
        public string Symbol { get; set; } = "R";
        public string IsoCode { get; set; } = "ZAR";
        public string Name { get; set; } = "Rand";
        public int Decimals { get; set; } = 2;
        public string DecimalSeparator { get; set; } = ".";
        public string ThousandSeparator { get; set; } = "";
    }

    public class GameInfo
    {
        public decimal Rtp { get; set; } = 96.0m;
        public string Mode { get; set; } = "real";           // real|demo
        public BetConfig Bet { get; set; } = new();
        public bool FunMode { get; set; }
        public decimal MaxWinCap { get; set; } = 0m;
        public GameConfig Config { get; set; } = new();
        public FreeSpinsInfo? FreeSpins { get; set; }
        public FreeSpinsInfo? PromoFreeSpins { get; set; }
        public FeatureInfo? Feature { get; set; }
        public LastPlayInfo? LastPlay { get; set; }
    }

    public class BetConfig
    {
        public decimal Default { get; set; } = 1m;
        public List<decimal> Levels { get; set; } = new() { 1m, 2m, 5m };
    }

    public class GameConfig
    {
        public string StartScreen { get; set; } = "main";
        public Dictionary<string, object> Settings { get; set; } = new();
    }

    public class FreeSpinsInfo
    {
        public int Available { get; set; }
        public int? Total { get; set; }
        public decimal? Multiplier { get; set; }
    }

    public class FeatureInfo
    {
        public string Name { get; set; } = "none";
        public string Type { get; set; } = "none";
        public bool? IsClosure { get; set; }
    }

    public class LastPlayInfo
    {
        public BetLevel BetLevel { get; set; } = new();
        public object? Results { get; set; }
    }

    public class BetLevel
    {
        public int Index { get; set; } = 0;
        public decimal Value { get; set; } = 1m;
    }

    // --- Play response specific ---

    public class PlayerRound
    {
        public string SessionId { get; set; } = string.Empty;
        public string RoundId { get; set; } = string.Empty;
        public TransactionBreakdown Transaction { get; set; } = new();
        public decimal PrevBalance { get; set; }
        public decimal Balance { get; set; }
        public decimal Bet { get; set; }
        public decimal Win { get; set; }
        public string CurrencyId { get; set; } = "ZAR";
    }

    public class TransactionBreakdown
    {
        public decimal Withdraw { get; set; }
        public decimal Deposit { get; set; }
    }

    public class GameRound
    {
        public object? Results { get; set; }
        public string Mode { get; set; } = "real";
        public MaxWinCap MaxWinCap { get; set; } = new();
    }

    public class MaxWinCap
    {
        public bool Achieved { get; set; }
        public decimal Value { get; set; }
        public decimal RealWin { get; set; }
    }

    public class Jackpot
    {
        public string Name { get; set; } = "Base";
        public decimal Amount { get; set; }
    }
}
