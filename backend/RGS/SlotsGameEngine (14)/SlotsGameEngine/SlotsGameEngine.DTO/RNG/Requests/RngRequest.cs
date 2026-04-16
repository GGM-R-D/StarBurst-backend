using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotsGameEngine.DTO.Rng.Requests
{
    public class RngRequest
    {
        public int? Min { get; set; }
        public int? Max { get; set; }
        public string? GameId { get; set; }
        public string? RoundId { get; set; }
    }
}
