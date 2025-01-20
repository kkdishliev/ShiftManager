using Newtonsoft.Json;
using ShiftManager.Application.DTOs;
using System.Linq.Expressions;
using System.Reflection;

namespace ShiftManager.Application.Services
{
    public class QueryService : IQueryService
    {
        public IQueryable<T> ApplyFiltersAndSorting<T>(
            IQueryable<T> query,
            string? globalFilter = null,
            string? filters = null,
            string? sorting = null)
            where T : class
        {
            if (!string.IsNullOrEmpty(globalFilter))
            {
                query = ApplyGlobalFilter(query, globalFilter);
            }

            if (!string.IsNullOrEmpty(filters))
            {
                query = ApplyCustomFilters(query, filters);
            }

            if (!string.IsNullOrEmpty(sorting))
            {
                query = ApplySorting(query, sorting);
            }

            return query;
        }


        private IQueryable<T> ApplyGlobalFilter<T>(IQueryable<T> query, string globalFilter) where T : class
        {
            var properties = typeof(T).GetProperties()
                .Where(p => p.PropertyType == typeof(string)); 

            foreach (var property in properties)
            {
                var parameter = Expression.Parameter(typeof(T), "x");
                var propertyAccess = Expression.Property(parameter, property);
                var method = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                var containsExpression = Expression.Call(propertyAccess, method, Expression.Constant(globalFilter));

                var lambda = Expression.Lambda<Func<T, bool>>(containsExpression, parameter);
                query = query.Where(lambda);
            }

            return query;
        }

        private IQueryable<T> ApplyCustomFilters<T>(IQueryable<T> query, string filters) where T : class
        {
            var filterCriteria = JsonConvert.DeserializeObject<List<FilterCriteria>>(filters);

            foreach (var filter in filterCriteria)
            {
                var property = typeof(T).GetProperty(filter.Id, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                if (property != null)
                {
                    var parameter = Expression.Parameter(typeof(T), "x");
                    var propertyAccess = Expression.Property(parameter, property);
                    var filterValue = Convert.ChangeType(filter.Value, property.PropertyType);
                    var constantExpression = Expression.Constant(filterValue);
                    var equalityExpression = Expression.Equal(propertyAccess, constantExpression);

                    var lambda = Expression.Lambda<Func<T, bool>>(equalityExpression, parameter);
                    query = query.Where(lambda);
                }
                //else
                //{
                //    Console.WriteLine($"Property '{filter.Id}' not found in type '{typeof(T).Name}'.");
                //}
            }

            return query;
        }

        private IQueryable<T> ApplySorting<T>(IQueryable<T> query, string sorting) where T : class
        {
            var sortingCriteria = JsonConvert.DeserializeObject<List<SortingCriteria>>(sorting);

            foreach (var sort in sortingCriteria)
            {
                var property = typeof(T).GetProperty(sort.Id, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                if (property != null)
                {
                    var parameter = Expression.Parameter(typeof(T), "x");
                    var propertyAccess = Expression.Property(parameter, property);
                    var lambda = Expression.Lambda<Func<T, object>>(propertyAccess, parameter);

                    query = sort.Desc ? query.OrderByDescending(lambda) : query.OrderBy(lambda);
                }
                //else
                //{
                //    Console.WriteLine($"Property '{sort.Id}' not found in type '{typeof(T).Name}'.");
                //}
            }

            return query;
        }
    }
}