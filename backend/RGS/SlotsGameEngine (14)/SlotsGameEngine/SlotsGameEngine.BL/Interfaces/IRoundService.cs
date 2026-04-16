using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotsGameEngine.BL.BLL.Interfaces
{
    public interface IRoundService
    {
        Task<string> GenerateRoundIdAsync();
    }
}
