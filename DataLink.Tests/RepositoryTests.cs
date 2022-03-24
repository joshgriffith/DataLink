using System.Collections.Generic;
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
        public void ShouldQuery_InMemory() {
            var collection = new List<Person> {
                new () { Name = "Test1" },
                new () { Name = "Test2" },
            };

            var hub = DataHubConfiguration.Default()
                .Use(()=> collection.AsQueryable())
                .CreateHub();

            var results = hub.Get<Person>().ToList();
            
            Assert.AreEqual(2, results.Count);
        }
        
        [TestMethod]
        public void ShouldQuery_Simple() {
            var configuration = DataHubConfiguration.Default();
            configuration.Use(() => new PersonRepository().Get());

            var hub = DataHub.FromConfiguration(configuration);
            var results = hub.Get<Person>().ToList();
            
            Assert.AreEqual(4, results.Count);
        }

        [TestMethod]
        public void ShouldQuery_Count() {
            var configuration = DataHubConfiguration.Default();
            configuration.Use(() => new PersonRepository().Get());

            var hub = DataHub.FromConfiguration(configuration);
            var result = hub.Get<Person>().Count();
            
            Assert.AreEqual(4, result);
        }

        [TestMethod]
        public void ShouldQuery_InterfaceAggregation() {
            var configuration = DataHubConfiguration.Default();
            configuration.Use<PersonRepository>();
            configuration.Use<PetRepository>();

            var hub = DataHub.FromConfiguration(configuration);

            var results = hub.Get<HasAge>()
                .Where(each => each.Age == 8)
                .ToList();

            Assert.AreEqual(2, results.Count);
        }

        [TestMethod]
        public void ShouldQuery_InterfaceProjection() {
            var configuration = DataHubConfiguration.Default();
            configuration.Use<PersonRepository>();
            configuration.Use<PetRepository>();

            var hub = DataHub.FromConfiguration(configuration);

            var results = hub.Get<HasAge>()
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