using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotsGameEngine.DTO.Shared.DataObjects
{
    public class TokenData
    {
        public long PlayerId { get; set; }
        public long OperatorId { get; set; }
        public string Currency { get; set; } = "EUR";
        public string RawToken { get; set; } = string.Empty;
    }
}
