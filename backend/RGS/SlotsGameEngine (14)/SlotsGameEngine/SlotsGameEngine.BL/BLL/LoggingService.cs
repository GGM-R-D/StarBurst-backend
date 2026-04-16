using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SlotsGameEngine.BL.BLL.Interfaces;
using SlotsGameEngine.BL.DAL;

namespace SlotsGameEngine.BL.BLL
{
    public class LoggingService : ILoggingService
    {
        private readonly ApiLogRepository _repository;

        public LoggingService(ApiLogRepository repository)
        {
            _repository = repository;
        }

        public Task LogApiCallAsync(
            string endpoint,
            string method,
            string? requestBody,
            string? responseBody,
            int? statusCode,
            string? error,
            CancellationToken ct)
        {
            return _repository.LogAsync(endpoint, method, requestBody, responseBody, statusCode, error, ct);
        }
    }
}