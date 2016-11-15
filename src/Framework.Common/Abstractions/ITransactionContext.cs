using System.Threading.Tasks;

namespace Framework.Common
{
    public interface ITransactionContext
    {
        void BeginTransaction();
        Task CommitTransactionAsync();
        void RollbackTransaction();
    }
}
