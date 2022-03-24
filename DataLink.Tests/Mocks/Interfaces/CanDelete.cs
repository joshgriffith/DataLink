using System;

namespace DataLink.Tests.Mocks.Interfaces {
    public interface CanDelete {
        DateTime? DeletedDate { get; set; }
    }
}