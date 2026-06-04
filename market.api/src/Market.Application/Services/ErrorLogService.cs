using Market.Application.Interfaces;
using Market.Domain.Entities;
using Market.Domain.Enums;
using Market.Domain.Interfaces;

namespace Market.Application.Services
{
    public class ErrorLogService : IErrorLogService
    {
        private readonly IErrorLogRepository _repository;

        public ErrorLogService(IErrorLogRepository repository)
        {
            _repository = repository;
        }

        public async Task AddAsync(Exception exception, string source, string requestPath, string httpMethod)
        {
            var errorLog = new ErrorLog
            {
                Id = Guid.NewGuid(),
                Source = source,
                Message = exception.Message,
                StackTrace = exception.StackTrace ?? string.Empty,
                RequestPath = requestPath,
                HttpMethod = httpMethod,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Status = StatusEnum.Active
            };

            await _repository.AddAsync(errorLog);
        }
    }
}
