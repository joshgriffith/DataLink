using System;
using System.Linq;
using System.Linq.Expressions;
using DataLink.Core;
using DataLink.Core.Configuration;
using DataLink.Core.Filters;
using DataLink.Tests.Mocks;
using DataLink.Tests.Mocks.Interfaces;
using DataLink.Tests.Mocks.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DataLink.Tests {
    
    [TestClass]
    public class FilterTests {
        
        [TestMethod]
        public void OneFilter() {
            var configuration = DataHubConfiguration.Default();

            configuration.Use(() => new PersonRepository().Get());
            configuration.Use<MarriageFilter>();
            
            var hub = DataHub.FromConfiguration(configuration);
            var results = hub.Get<Person>().ToList();
            
            Assert.AreEqual(2, results.Count);
            Assert.IsTrue(results.All(each => each.IsMarried));
        }

        [TestMethod]
        public void MultipleFilters() {
            var configuration = DataHubConfiguration.Default();

            configuration.Use(() => new PersonRepository().Get());
            configuration.Use<MarriageFilter>();
            configuration.Use<SingleFilter>();
            
            var hub = DataHub.FromConfiguration(configuration);
            var results = hub.Get<Person>().ToList();
            
            Assert.AreEqual(4, results.Count);
        }

        [TestMethod]
        public void MultipleFiltersReversed() {
            var configuration = DataHubConfiguration.Default();

            configuration.Use(() => new PersonRepository().Get());
            configuration.Use<SingleFilter>();
            configuration.Use<MarriageFilter>();
            
            var hub = DataHub.FromConfiguration(configuration);
            var results = hub.Get<Person>().ToList();
            
            Assert.AreEqual(4, results.Count);
        }

        [TestMethod]
        public void ShouldFilter_SingleInterface() {
            var configuration = DataHubConfiguration.Default();

            configuration.Use(() => new PersonRepository().Get());
            configuration.Filter<CanDelete>(query => query.DeletedDate == null || DateTime.UtcNow < query.DeletedDate);
            
            var hub = DataHub.FromConfiguration(configuration);
            var results = hub.Get<Person>().ToList();
            
            Assert.AreEqual(3, results.Count);
        }

        [TestMethod]
        public void ShouldFilter_MultipleInterfaces() {
            var configuration = DataHubConfiguration.Default();

            configuration.Use(() => new PersonRepository().Get());
            configuration.Filter<Person>(query => !query.IsMarried);
            configuration.Filter<HasAge>(query => query.Age == 34);

            var hub = DataHub.FromConfiguration(configuration);
            var results = hub.Get<Person>().ToList();
            
            Assert.AreEqual(3, results.Count);
        }

        [TestMethod]
        public void ShouldFilter_WithPrequisite() {
            var configuration = DataHubConfiguration.Default();

            configuration.Use(() => new PersonRepository().Get());

            configuration.Use<MarriageFilter>();
            configuration.Filter<HasAge>(query => query.Age == 38);
            configuration.Use<DeletionQueryFilter>();

            var hub = DataHub.FromConfiguration(configuration);
            var results = hub.Get<Person>().ToList();
            
            Assert.AreEqual(2, results.Count);
        }

        [TestMethod]
        public void ShouldFilter_WithBaseClass() {
            var configuration = DataHubConfiguration.Default();

            configuration.Use(() => new PersonRepository().Get());
            configuration.Filter<BaseEntity>(x => x.DeletedDate == null || DateTime.UtcNow < x.DeletedDate);

            var hub = DataHub.FromConfiguration(configuration);
            var results = hub.Get<Person>().ToList();
            
            Assert.AreEqual(3, results.Count);
        }
        
        protected class MarriageFilter : QueryFilter<Person> {
            protected override Expression<Func<Person, bool>> GetFilterExpression() {
                return person => person.IsMarried == true;
            }
        }

        protected class SingleFilter : QueryFilter<Person> {
            protected override Expression<Func<Person, bool>> GetFilterExpression() {
                return person => person.IsMarried == false;
            }
        }

        protected class DeletionQueryFilter : QueryFilter<CanDelete> {

            public DeletionQueryFilter()
                : base(QueryFilterTypes.Exclusive) {
            }

            protected override Expression<Func<CanDelete, bool>> GetFilterExpression() {
                return each => each.DeletedDate != null && each.DeletedDate <= DateTime.UtcNow;
            }
        }
    }
}