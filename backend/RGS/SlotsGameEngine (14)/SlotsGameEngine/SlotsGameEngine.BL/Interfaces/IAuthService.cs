using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SlotsGameEngine.DTO.Shared.DataObjects;

namespace SlotsGameEngine.BL.BLL.Interfaces
{
    public interface IAuthService
    {
        Task<TokenData> ValidateTokenAsync(string token, CancellationToken ct);
    }
}
