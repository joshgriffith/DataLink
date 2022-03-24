using System.Linq;
using DataLink.Core;
using DataLink.Core.Configuration;
using DataLink.Tests.Mocks;
using DataLink.Tests.Mocks.Interfaces;
using DataLink.Tests.Mocks.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DataLink.Tests {
    
    [TestClass]
    public class RepositoryTests {
        
        [TestMethod]
        public void ShouldQuery_Simple() {
            var configuration = DataHubConfiguration.Default();
            configuration.Use(() => new PersonRepository().Get());

            var provider = DataHub.FromConfiguration(configuration);
            var results = provider.Get<Person>().ToList();
            
            Assert.AreEqual(4, results.Count);
        }

        [TestMethod]
        public void ShouldQuery_Count() {
            var configuration = DataHubConfiguration.Default();
            configuration.Use(() => new PersonRepository().Get());

            var provider = DataHub.FromConfiguration(configuration);
            var result = provider.Get<Person>().Count();
            
            Assert.AreEqual(4, result);
        }

        [TestMethod]
        public void ShouldQuery_InterfaceAggregation() {
            var configuration = DataHubConfiguration.Default();
            configuration.Use<PersonRepository>();
            configuration.Use<PetRepository>();

            var provider = DataHub.FromConfiguration(configuration);

            var results = provider.Get<HasAge>()
                .Where(each => each.Age == 8)
                .ToList();

            Assert.AreEqual(2, results.Count);
        }

        [TestMethod]
        public void ShouldQuery_InterfaceProjection() {
            var configuration = DataHubConfiguration.Default();
            configuration.Use<PersonRepository>();
            configuration.Use<PetRepository>();

            var provider = DataHub.FromConfiguration(configuration);

            var results = provider.Get<HasAge>()
                .Where(each => each.Age == 8)
                .Select(each => new {
                    Test = each.Age
                })
                .ToList();

            Assert.AreEqual(2, results.Count);
        }

        //[TestMethod]
        //public void Should_QueryWithNaturalLanguage() {
            // await DataHub.Query<SessionFile>().Use<NaturalLanguage>("sessions created during a holiday").ToListAsync();
        //}
    }
}