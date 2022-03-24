using System.Collections.Generic;
using DataLink.Tests.Mocks.Interfaces;

namespace DataLink.Tests.Mocks {
    public class Person : BaseEntity, HasName, HasAge, HasPets {
        public string Name { get; set; }
        public int Age { get; set; }
        public bool IsMarried { get; set; }
        public List<Pet> Pets { get; set; }
    }
}