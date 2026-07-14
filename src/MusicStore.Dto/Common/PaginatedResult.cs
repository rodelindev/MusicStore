namespace MusicStore.Dto.Common;

public class PaginatedResult<T>
{
    public IReadOnlyList<T> Data { get; set; } = [];
    public PaginationMeta Meta { get; set; } = new();
}

public class PaginationMeta
{
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => PageSize > 0 ? (int)Math.Ceiling(TotalCount / (double)PageSize) : 0;
}