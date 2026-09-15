namespace JobTracker.Application.DTOs.Common
{
    public class PagedResultDto<T>
    {
        public IReadOnlyList<T> Items { get; set; } = [];

        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public int TotalCount { get; set; }

        public int TotalPages =>
                PageSize <= 0 ? 0
                    : (int)Math.Ceiling((double)TotalCount / PageSize);

        public bool HasPrevious =>
            PageNumber > 1;

        public bool HasNext =>
            PageNumber < TotalPages;
    }
}