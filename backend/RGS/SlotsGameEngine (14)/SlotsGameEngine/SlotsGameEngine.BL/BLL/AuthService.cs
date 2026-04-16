using System.Threading;
using System.Threading.Tasks;
using SlotsGameEngine.BL.BLL.Interfaces;
using SlotsGameEngine.BL.Helpers;
using SlotsGameEngine.DTO.Shared.DataObjects;

namespace SlotsGameEngine.BL.BLL
{
    public class AuthService : IAuthService
    {
        private readonly TokenParser _parser;

        public AuthService(TokenParser parser)
        {
            _parser = parser;
        }

        public Task<TokenData> ValidateTokenAsync(string token, CancellationToken ct)
        {
            var parsed = _parser.Parse(token);
            return Task.FromResult(parsed);
        }
    }
}
