using System.Collections.Generic;
using System.Linq;

namespace DataLink.Core.Aggregation {
    public static class EnumerableAggregators {
        public static IEnumerable<T> Aggregate<T>(IEnumerable<IEnumerable<T>> items) {
            return items.SelectMany(x => x).ToList();
        }
    }
}