using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SlotsGameEngine.BL.BLL.Interfaces;

namespace SlotsGameEngine.BL.BLL
{
    public class RoundService : IRoundService
    {
        public Task<string> GenerateRoundIdAsync()
        {
            return Task.FromResult(Guid.NewGuid().ToString("N"));
        }
    }
}
