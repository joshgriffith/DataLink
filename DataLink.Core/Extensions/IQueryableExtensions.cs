using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace DataLink.Core.Extensions {
    public static class IQueryableExtensions {
        
        public static async Task<IList> ToListAsync(this IQueryable queryable) {
            var type = queryable.GetType().GetGenericType();
            var task = typeof(IQueryableExtensions).InvokeExtensionMethod(nameof(ToListAsync), queryable, type) as Task;
            await task;
            return task.GetType().GetProperty("Result").GetValue(task) as IList;
        }

        public static async Task<List<T>> ToListAsync<T>(this IQueryable<T> queryable, CancellationToken token = default) {
            if (queryable is IAsyncQueryable<T> enumerable)
                return await enumerable.ToListAsync(token);

            return queryable.ToList();
        }

        public static async Task<T> FirstOrDefaultAsync<T>(this IQueryable<T> queryable) {
            if (queryable is IAsyncQueryable<T> enumerable)
                return await enumerable.FirstOrDefaultAsync();

            return queryable.FirstOrDefault();
        }

        public static async Task<T> FirstOrDefaultAsync<T>(this IQueryable<T> queryable, Expression<Func<T, bool>> predicate) {
            if (queryable is IAsyncQueryable<T> query)
                return await query.FirstOrDefaultAsync(predicate);

            return queryable.FirstOrDefault(predicate);
        }
    }
}