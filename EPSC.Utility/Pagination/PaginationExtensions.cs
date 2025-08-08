using Microsoft.EntityFrameworkCore;


namespace EPSC.Utility.Pagination
{
    public static class PaginationExtensions
    {
        public static async Task<PagedResponse<T>> ToPagedResponseAsync<T>(this IQueryable<T> source, int pageNumber, int pageSize)
        {
            var count = await source.CountAsync();
            var data = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PagedResponse<T>(data, count, pageNumber, pageSize);
        }
    }
}
