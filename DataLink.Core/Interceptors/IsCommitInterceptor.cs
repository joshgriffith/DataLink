using System.Threading.Tasks;
using DataLink.Core.ChangeTracking;

namespace DataLink.Core.Interceptors {
    public interface IsCommitInterceptor {
        Task BeforeCommit(ChangeSet changes);
        Task AfterCommit(ChangeSet changes);
    }
}