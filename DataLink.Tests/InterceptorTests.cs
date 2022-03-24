using System;
using System.Linq;
using System.Threading.Tasks;
using DataLink.Core;
using DataLink.Core.Configuration;
using DataLink.Core.Extensions;
using DataLink.Core.Interceptors;
using DataLink.Tests.Mocks;
using DataLink.Tests.Mocks.Interfaces;
using DataLink.Tests.Mocks.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DataLink.Tests {
    
    [TestClass]
    public class InterceptorTests {

        [TestMethod]
        public async Task Should_Intercept_Load() {
            var configuration = DataHubConfiguration.Default()
                .Use<PetRepository>()
                .Use<PersonRepository>()
                .Use<OverridePetAge>()
                .Use<OverridePersonAge>();

            var provider = DataHub.FromConfiguration(configuration);
            var results = await provider.Get<HasAge>().ToListAsync();

            foreach(var person in results.OfType<Person>())
                Assert.AreEqual(1, person.Age);

            foreach(var pet in results.OfType<Pet>())
                Assert.AreEqual(2, pet.Age);
        }

        [TestMethod]
        public async Task Should_Intercept_Save() {
            var date = DateTime.Today;
            const string petName = "Fido";
            const string personName = "John";

            var configuration = DataHubConfiguration.Default()
                .Use<PetRepository>()
                .Use<PersonRepository>()
                .Use(()=> new PersonAgeInterceptor(21))
                .Use(()=> new PetNameInterceptor(petName))
                .Use(()=> new CreationDateInterceptor(date));

            var data = DataHub.FromConfiguration(configuration);
            var pet = await data.SaveAsync(new Pet());

            Assert.AreEqual(petName, pet.Name);
            Assert.AreEqual(date, pet.CreationDate);

            var person = await data.SaveAsync(new Person { Name = personName });
            Assert.AreEqual(personName, person.Name);
            Assert.AreEqual(date, person.CreationDate);
        }

        internal class OverridePersonAge : LoadInterceptor<Person> {
            
            protected override void OnLoad(Person person) {
                person.Age = 1;
            }
        }

        internal class OverridePetAge : LoadInterceptor<Pet> {
            protected override void OnLoad(Pet pet) {
                pet.Age = 2;
            }
        }

        protected class PersonAgeInterceptor : SaveInterceptor<Person> {
            private readonly int _age;

            public PersonAgeInterceptor(int age) {
                _age = age;
            }

            protected override async Task OnBeforeSave(Person person) {
                person.Age = _age;
            }
        }

        protected class PetNameInterceptor : SaveInterceptor<Pet> {
            private readonly string _name;

            public PetNameInterceptor(string name) {
                _name = name;
            }

            protected override async Task OnBeforeSave(Pet pet) {
                pet.Name = _name;
            }
        }

        protected class CreationDateInterceptor : SaveInterceptor<HasCreationDate> {

            private readonly DateTime _date;
            
            public CreationDateInterceptor(DateTime date) {
                _date = date;
            }

            protected override async Task OnBeforeSave(HasCreationDate entity) {
                if (entity.CreationDate == DateTime.MinValue)
                    entity.CreationDate = _date;
            }
        }
    }
}