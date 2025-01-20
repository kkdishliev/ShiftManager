namespace ShiftManager.Common.Extensions
{
    public static class IQueryableExtensions
    {
        public static IQueryable<T> Page<T>(this IQueryable<T> query, int start, int size)
        {
            return query.Skip(start).Take(size);
        }
    }
}