using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotsGameEngine.DTO.Shared.Enums
{
    public enum ErrorCode
    {
        None = 0,
        InvalidToken = 1,
        InsufficientBalance = 2,
        UpstreamError = 3,
        ValidationError = 4
    }
}