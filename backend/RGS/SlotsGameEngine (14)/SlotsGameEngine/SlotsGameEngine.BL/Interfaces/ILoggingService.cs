using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SlotsGameEngine.BL.BLL.Interfaces
{
    public interface ILoggingService
    {
        Task LogApiCallAsync(
            string endpoint,
            string method,
            string? requestBody,
            string? responseBody,
            int? statusCode,
            string? error,
            CancellationToken ct);
    }
}
