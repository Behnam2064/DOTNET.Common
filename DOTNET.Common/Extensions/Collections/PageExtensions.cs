using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DOTNET.Common.Extensions.Collections
{
    public static class PageExtensions
    {
        public static IQueryable<T> Page<T>(this IQueryable<T> Query, int Page, int PageSize)
        {
            int skip = Math.Max(PageSize * (Page - 1), 0);
            return Query.Skip(skip).Take(PageSize);
        }
    }
}
