using Microsoft.EntityFrameworkCore;
using MyRoboAdvisor.Database.Data;

namespace MyRoboAdvisor.Database.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    private readonly RoboAdvisorDbContext _context;
    private readonly DbSet<T> _dbSet;

    public GenericRepository(RoboAdvisorDbContext context)
    {
        _context = context;
        _dbSet = _context.Set<T>();
    }

    public IQueryable<T> Table => _dbSet;

    public IQueryable<T> TableAsNoTracking => _dbSet.AsNoTracking();

    public async Task BeginTransactionAsync(CancellationToken cancellationToken)
    {
        await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken)
    {
        await _context.Database.CommitTransactionAsync(cancellationToken);
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken)
    {
        await _context.Database.RollbackTransactionAsync(cancellationToken);
    }

    public void Delete(T entity)
    {
        _dbSet.Remove(entity);
    }

    public void DeleteRange(List<T> entity)
    {
        _dbSet.RemoveRange();
    }

    public async Task InsertAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
    }

    public void InsertRange(List<T> entities)
    {
        _dbSet.AddRange(entities);
    }

    public void Update(T entity)
    {
        _dbSet.Update(entity);
    }

    public void UpdateRange(List<T> entity)
    {
        _dbSet.UpdateRange(entity);
    }

    public async Task SaveAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}

