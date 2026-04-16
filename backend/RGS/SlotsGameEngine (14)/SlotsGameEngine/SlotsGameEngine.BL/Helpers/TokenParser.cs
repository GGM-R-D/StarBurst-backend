using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SlotsGameEngine.DTO.Shared.DataObjects;

namespace SlotsGameEngine.BL.Helpers
{
    public class TokenParser
    {
        public TokenData Parse(string token)
        {
            // TODO: Replace with real token parsing logic.
            return new TokenData
            {
                PlayerId = 123,
                OperatorId = 1,
                Currency = "ZAR",
                RawToken = token
            };
        }
    }
}
