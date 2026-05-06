namespace User.Core.Responses
{
    public class PaginatedResponse<T>
    {
        public dynamic Request { get; set; }
        public List<T> Items { get; set; }
        public int TotalPages  => (int)Math.Ceiling(TotalItems / (double)PageSize);
        public int TotalItems { get; set; }
        public bool HasPreviousPage  => PageIndex > 1;
        public bool HasNextPage => PageIndex < TotalPages;
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        public PaginatedResponse(List<T> items, int totalItems, int pageIndex, int pageSize, dynamic request)
        {
            Items = items;
            Request = request;
            TotalItems = totalItems;
            PageIndex = pageIndex;
            PageSize = pageSize;
        }
    }
}
