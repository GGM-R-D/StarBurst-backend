using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.Json;

namespace SlotsGameEngine.DTO.Client.Requests
{
    public class PlayRequest
    {
        public string SessionId { get; set; } = string.Empty;   // not nullable
        public List<BetLine> Bets { get; set; } = new();        // not nullable
        public JsonElement? UserPayload { get; set; }            // may be null
    }

    public class BetLine
    {
        public decimal Amount { get; set; }
        public Dictionary<string, object>? Meta { get; set; }   // may be null
    }
}
