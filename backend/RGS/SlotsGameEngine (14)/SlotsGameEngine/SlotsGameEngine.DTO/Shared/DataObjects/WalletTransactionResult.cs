using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotsGameEngine.DTO.Shared.DataObjects
{
    public class WalletTransactionResult
    {
        public bool Success { get; set; }
        public decimal Balance { get; set; }
        public string Currency { get; set; } = "EUR";
        public string? Error { get; set; }
    }
}
