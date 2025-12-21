using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using SEP490_FTCDHMM_API.Application.Interfaces.SystemServices;
using SEP490_FTCDHMM_API.Infrastructure.Data;

namespace SEP490_FTCDHMM_API.Infrastructure.Persistence
{
    public class UnitOfWork : IUnitOfWork, IDisposable, IAsyncDisposable
    {
        private readonly AppDbContext _context;
        private IDbContextTransaction? _currentTransaction;
        private readonly List<Func<Task>> _afterCommitActions = new();
        private readonly ILogger<UnitOfWork> _logger;

        public UnitOfWork(AppDbContext context, ILogger<UnitOfWork> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task BeginTransactionAsync()
        {
            if (_currentTransaction != null)
                return;

            _currentTransaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            if (_currentTransaction == null)
                return;

            await _currentTransaction.CommitAsync();
            await _currentTransaction.DisposeAsync();
            _currentTransaction = null;

            foreach (var action in _afterCommitActions)
            {
                try
                {
                    await action();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error executing after-commit action");
                }
            }

            _afterCommitActions.Clear();
        }

        public async Task RollbackTransactionAsync()
        {
            if (_currentTransaction == null)
                return;

            await _currentTransaction.RollbackAsync();
            await _currentTransaction.DisposeAsync();
            _currentTransaction = null;

            _afterCommitActions.Clear();
        }

        public async Task ExecuteInTransactionAsync(Func<Task> operation)
        {
            await BeginTransactionAsync();

            try
            {
                await operation();

                await _context.SaveChangesAsync();

                await CommitTransactionAsync();
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("Transaction canceled by request token.");
                await RollbackTransactionAsync();
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing transaction, rolling back...");
                await RollbackTransactionAsync();
                throw;
            }
        }

        public void RegisterAfterCommit(Func<Task> action)
        {
            _afterCommitActions.Add(action);
        }

        public void Dispose()
        {
            _currentTransaction?.Dispose();
            _afterCommitActions.Clear();
        }

        public async ValueTask DisposeAsync()
        {
            if (_currentTransaction != null)
                await _currentTransaction.DisposeAsync();

            _afterCommitActions.Clear();
        }
    }
}
