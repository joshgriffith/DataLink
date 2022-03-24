using System.Collections.Generic;
using System.Linq;
using DataLink.Core.Repositories;

namespace DataLink.Tests.Mocks.Repositories {
    public class PetRepository : DefaultRepositoryFactory<Pet> {
        
        public PetRepository()
            : base(()=> new List<Pet> {
                new () { Id = 1, Age = 5 },
                new () { Id = 2, Age = 3 },
                new () { Id = 3, Age = 8 }
            }.AsQueryable()) {
        }
    }
}