namespace Market.Application.Interfaces
{
    public interface IErrorLogService
    {
        Task AddAsync(Exception exception, string source, string requestPath, string httpMethod);
    }
}
