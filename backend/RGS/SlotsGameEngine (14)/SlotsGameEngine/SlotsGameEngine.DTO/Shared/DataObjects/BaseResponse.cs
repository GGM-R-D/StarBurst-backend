using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotsGameEngine.DTO.Shared.DataObjects
{
    public class BaseResponse
    {
        public int StatusCode { get; set; } = 6000;   // 6000 = OK (per spec)
        public string Message { get; set; } = "OK";
    }
}