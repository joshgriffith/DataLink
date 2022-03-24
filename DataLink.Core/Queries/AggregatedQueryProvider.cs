using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DataLink.Core.Aggregation;

namespace DataLink.Core.Queries {
    public class AggregatedQueryProvider : IQueryProvider {
        
        public readonly List<IQueryable> Queryables;
        private readonly Action<IEnumerable> _callback;

        public AggregatedQueryProvider(Action<IEnumerable> callback, params IQueryable[] queries) {
            _callback = callback;
            Queryables = queries.ToList();
        }

        public IQueryable CreateQuery(Expression expression) {
            var type = expression.Type.GetElementType();

            try {
                return (IQueryable) Activator.CreateInstance(typeof(AggregatedQuery<>).MakeGenericType(type), this, expression);
            }
            catch (System.Reflection.TargetInvocationException exception) {
                throw exception.InnerException;
            }
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression) {
            return new AggregatedQuery<TElement>(this, expression);
        }

        public object Execute(Expression expression) {
            throw new NotImplementedException("Todo: AggregatedQueryProvider.Execute");
        }

        public TResult Execute<TResult>(Expression expression) {

            var results = Queryables.Select(query => {
                var visitor = new AggregatedQueryRebinder(query);
                var newExpression = visitor.Visit(expression);
                return query.Provider.Execute<TResult>(newExpression);
            }).ToList(); // ToList is required here for type checking in Aggregator (todo: fix and clean this up)
            
            return Aggregator.Aggregate(results, _callback);
        }
    }
}