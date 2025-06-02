using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Spesification
{
    public static class PaginationSpec
    {
        public static async Task<List<T>> ToPaginateResponseAsync<T>(IQueryable<T> query, int? pageNumber, int? pageSize)
        {
            var page = (pageNumber.HasValue && pageNumber > 0) ? pageNumber.Value : 1;
            var size = (pageSize.HasValue && pageSize > 0) ? pageSize.Value : 10;

            return await query
                .Skip((page - 1) * size)
                .Take(size)
                .ToListAsync();
        }
    }
}
