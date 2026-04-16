using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotsGameEngine.DTO.Client.Requests
{
    // Spec: playerId (from Start response)
    public class BalanceRequest
    {
        public string PlayerId { get; set; } = string.Empty;
    }
}