using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SlotsGameEngine.DTO.Shared.DataObjects
{
    public class BetResult
    {
        public string RoundId { get; set; } = "";
        public decimal WinAmount { get; set; }
        public decimal BetAmount { get; set; }          // keep if you use it
        public Dictionary<string, object?>? Payload { get; set; }  // ← allow nulls
    }
}
