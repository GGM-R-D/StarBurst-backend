using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlotsGameEngine.DTO.Shared.DataObjects;

namespace SlotsGameEngine.DTO.Client.Responses
{
    public class BalanceResponse : BaseResponse
    {
        public decimal Balance { get; set; }
    }
}

