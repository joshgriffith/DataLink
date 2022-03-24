using System;
using System.Collections.Generic;
using System.Linq;
using DataLink.Core.Repositories;

namespace DataLink.Tests.Mocks.Repositories {
    public class PersonRepository : DefaultRepositoryFactory<Person> {
        public PersonRepository()
            : base(()=> new List<Person> {
                new () { Id = 1, Name = "Josh", Age = 38 },
                new () { Id = 2, Name = "Matt", Age = 36, IsMarried = true, DeletedDate = DateTime.UtcNow.Subtract(TimeSpan.FromHours(1)) },
                new () { Id = 3, Name = "Chris", Age = 34, IsMarried = true },
                new () { Id = 3, Name = "Jonathan", Age = 8 }
            }.AsQueryable()) {
        }
    }
}