using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SlotsGameEngine.DTO.Shared.DataObjects;

namespace SlotsGameEngine.DTO.Client.Responses
{
    public class StartResponse : BaseResponse
    {
        public PlayerInfo Player { get; set; } = new();
        public ClientInfo Client { get; set; } = new();
        public CurrencyInfo Currency { get; set; } = new();
        public GameInfo Game { get; set; } = new();
    }
}

