using System.Collections.Generic;
using System.Linq;

namespace DataLink.Core.Aggregation {
    public static class NumericAggregators {
        public static int Aggregate(IEnumerable<int> items) {
            return items.Sum(x => x);
        }

        public static float Aggregate(IEnumerable<float> items) {
            return items.Sum(x => x);
        }

        public static double Aggregate(IEnumerable<double> items) {
            return items.Sum(x => x);
        }
    }
}