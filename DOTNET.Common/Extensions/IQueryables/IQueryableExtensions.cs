using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DOTNET.Common.Extensions.IQueryables
{
    public static class IQueryableExtensions
    {
        public static IQueryable<T> ApplySorting<T>(this IQueryable<T> source, string orderBy, string sortDirection)
        {
            if (string.IsNullOrWhiteSpace(orderBy))
                return source;

            var type = typeof(T);
            var property = type.GetProperty(orderBy);
            if (property == null)
                return source;

            var parameter = Expression.Parameter(type, "x");
            var selector = Expression.Property(parameter, property);
            var lambda = Expression.Lambda(selector, parameter);

            string method = sortDirection.ToLower() == "ascending" ? "OrderBy" : "OrderByDescending";

            var result = typeof(Queryable).GetMethods()
                .Where(m => m.Name == method && m.GetParameters().Length == 2)
                .Single()
                .MakeGenericMethod(type, property.PropertyType)
                .Invoke(null, new object[] { source, lambda });

            return (IQueryable<T>)result!;
        }

        public static IQueryable<T> ApplyPaging<T>(this IQueryable<T> query, int pageNumber, int pageSize)
        {
            return query.Skip((pageNumber - 1) * pageSize).Take(pageSize);
        }
    }

}
