namespace SEP490_FTCDHMM_API.Application.Interfaces.SystemServices
{
    public interface IUnitOfWork
    {
        Task BeginTransactionAsync(CancellationToken ct = default);
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
        Task ExecuteInTransactionAsync(Func<CancellationToken, Task> operation, CancellationToken ct = default);
        void RegisterAfterCommit(Func<Task> action);
    }
}
