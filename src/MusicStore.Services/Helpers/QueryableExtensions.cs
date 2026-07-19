using Microsoft.EntityFrameworkCore;
using MusicStore.Dto.Common;
using MusicStore.Entities;

namespace MusicStore.Services.Helpers;

public static class QueryableExtensions
{
    public static async Task<PaginatedResult<T>> ToPaginatedAsync<T>(
        this IQueryable<T> query,
        int page,
        int pageSize)
        where T : EntityBase<int>
    {
        var total = await query.CountAsync();
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PaginatedResult<T>
        {
            Data = items,
            Meta = new PaginationMeta
            {
                TotalCount = total,
                Page = page,
                PageSize = pageSize
            }
        };
    }
}