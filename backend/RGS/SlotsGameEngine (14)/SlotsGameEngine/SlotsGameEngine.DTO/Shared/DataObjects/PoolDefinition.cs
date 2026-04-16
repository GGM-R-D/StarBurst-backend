using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotsGameEngine.DTO.Shared.DataObjects
{
    public class PoolDefinition
    {
        public int Min { get; set; }
        public int Max { get; set; }
        public int Size { get; set; } = 1;
    }
}
