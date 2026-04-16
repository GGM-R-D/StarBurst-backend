using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotsGameEngine.DTO.Client.Requests
{
    // Spec: languageId, client, funMode, token (if real), currencyId (sometimes in fun)
    public class StartRequest
    {
        public string LanguageId { get; set; } = "en";
        public string Client { get; set; } = "desktop"; // "desktop" | "mobile"
        public int FunMode { get; set; } = 0;           // 0 = real, 1 = demo
        public string? Token { get; set; }              // required if FunMode=0
        public string? CurrencyId { get; set; }         // optional in demo
    }
}
