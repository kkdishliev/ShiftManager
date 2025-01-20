namespace ShiftManager.Application.Services
{
    public interface IQueryService
    {
        IQueryable<T> ApplyFiltersAndSorting<T>(
            IQueryable<T> query,
            string? globalFilter = null,
            string? filters = null,
            string? sorting = null)
            where T : class;
    }
}