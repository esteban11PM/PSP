using Microsoft.EntityFrameworkCore;

namespace School.API.Extensions;

public static class QueryableExtensions
{
    public static async Task<(IEnumerable<T> Items, int TotalCount)> ToPagedAsync<T>(
        this IQueryable<T> query, int page, int pageSize, CancellationToken ct = default)
    {
        if (page <= 0) page = 1;
        if (pageSize <= 0 || pageSize > 200) pageSize = 20;
        var total = await query.CountAsync(ct);
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
        return (items, total);
    }
}
