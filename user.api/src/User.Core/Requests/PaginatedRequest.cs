namespace User.Core.Requests
{
    public class PaginatedRequest
    {
        public string? Search { get; set; }
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public Guid? UserId { get; set; }
        public List<Guid>? UserIds { get; set; }
    }
}
