using Blockchain.Domain.Interfaces;
using Blockchain.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Blockchain.Infrastructure.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly AppDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public async Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default)
            => await _dbSet.FindAsync(new object[] { id }, ct);

        public async Task<IEnumerable<T>> GetAllAsync(CancellationToken ct = default)
            => await _dbSet.AsNoTracking().ToListAsync(ct);

        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default)
            => await _dbSet.AsNoTracking().Where(predicate).ToListAsync(ct);

        public async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default)
            => await _dbSet.AsNoTracking().FirstOrDefaultAsync(predicate, ct);

        public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default)
            => await _dbSet.AnyAsync(predicate, ct);

        public async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken ct = default)
            => predicate == null ? await _dbSet.CountAsync(ct) : await _dbSet.CountAsync(predicate, ct);

        public async Task<T> AddAsync(T entity, CancellationToken ct = default)
        {
            await _dbSet.AddAsync(entity, ct);
            return entity;
        }

        public async Task AddRangeAsync(IEnumerable<T> entities, CancellationToken ct = default)
            => await _dbSet.AddRangeAsync(entities, ct);

        public Task UpdateAsync(T entity, CancellationToken ct = default)
        {
            _dbSet.Update(entity);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(T entity, CancellationToken ct = default)
        {
            _dbSet.Remove(entity);
            return Task.CompletedTask;
        }

        public Task DeleteRangeAsync(IEnumerable<T> entities, CancellationToken ct = default)
        {
            _dbSet.RemoveRange(entities);
            return Task.CompletedTask;
        }

        public IQueryable<T> Query() => _dbSet.AsQueryable();

        public async Task<(IEnumerable<T> Items, int TotalCount)> GetPagedAsync(
            int page, int pageSize,
            Expression<Func<T, bool>>? predicate = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            CancellationToken ct = default)
        {
            var query = _dbSet.AsNoTracking();
            if (predicate != null) query = query.Where(predicate);
            var totalCount = await query.CountAsync(ct);
            if (orderBy != null) query = orderBy(query);
            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
            return (items, totalCount);
        }
    }
}
