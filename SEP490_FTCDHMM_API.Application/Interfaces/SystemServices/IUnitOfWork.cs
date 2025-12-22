namespace SEP490_FTCDHMM_API.Application.Interfaces.SystemServices
{
    public interface IUnitOfWork
    {
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
        Task ExecuteInTransactionAsync(Func<Task> operation);
        void RegisterAfterCommit(Func<Task> action);
    }
}
