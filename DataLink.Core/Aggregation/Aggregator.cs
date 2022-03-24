using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DataLink.Core.Extensions;

namespace DataLink.Core.Aggregation
{
    public static class Aggregator {

        public static T Aggregate<T>(IEnumerable<T> list, Action<IEnumerable> visitor) {

            var type = list.GetGenericType();

            if (list is IEnumerable<int> intList) {
                var result = NumericAggregators.Aggregate(intList);
                //visitor(new object[] { result });
                return (T) (object) result;
            }

            if (type.IsEnumerable()) {
                var innerType = type.GetGenericType();
                var method = typeof(EnumerableAggregators).GetMethod(nameof(Aggregate)).MakeGenericMethod(innerType);
                var items = (T) method.Invoke(null, new object [] { list });
                visitor(items as IEnumerable);
                return items;
            }

            var item = list.FirstOrDefault();

            if (item != null && !item.GetType().IsValueTypeOrString()) {
                visitor(new object[] { item });
            }

            return item;
        }
    }
}