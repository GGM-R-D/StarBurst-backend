using System.Threading;
using System.Threading.Tasks;

namespace SlotsGameEngine.BL.DAL
{
    public class ApiLogRepository
    {
        public Task LogAsync(
            string endpoint,
            string method,
            string? requestBody,
            string? responseBody,
            int? statusCode,
            string? error,
            CancellationToken ct)
        {
            // TODO: write to DB via DBHelper if you want logging.
            return Task.CompletedTask;
        }
    }
}
