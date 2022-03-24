using System;
using DataLink.Tests.Mocks.Interfaces;

namespace DataLink.Tests.Mocks {
    public abstract class BaseEntity : CanDelete, HasCreationDate {
        public int Id { get; set; }
        public DateTime? DeletedDate { get; set; }
        public DateTime CreationDate { get; set; }
    }
}