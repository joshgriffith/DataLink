using System.Collections.Generic;

namespace DataLink.Tests.Mocks.Interfaces {
    public interface HasPets {
        List<Pet> Pets { get; set; }
    }
}