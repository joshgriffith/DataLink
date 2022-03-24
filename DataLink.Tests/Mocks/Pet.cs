using DataLink.Tests.Mocks.Interfaces;

namespace DataLink.Tests.Mocks {
    public class Pet : BaseEntity, HasName, HasAge {
        public string Name { get; set; }
        public int Age { get; set; }
        public Person Owner { get; set; }
    }
}