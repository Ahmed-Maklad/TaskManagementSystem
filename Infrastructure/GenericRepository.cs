using Domain.Contracts;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Data;
namespace Infrastructure
{
    public class GenericRepository<TEntity, TId> : IGenericRepository<TEntity, TId> where TEntity : BaseEntity<TId>
    {
        private readonly AppDbContext context;
        private readonly DbSet<TEntity> dbset;
        public GenericRepository(AppDbContext _context)
        {
            context = _context;
            dbset = context.Set<TEntity>();
        }
        public async Task<TEntity> CreateAsync(TEntity Entity) => (await dbset.AddAsync(Entity)).Entity;

        public Task<TEntity> UpdateAsync(TEntity Entity) => Task.FromResult(dbset.Update(Entity).Entity);

        public Task<TEntity> DeleteAsync(TEntity Entity) => Task.FromResult(dbset.Remove(Entity).Entity);

        public Task<IQueryable<TEntity>> GetAllAsync() => Task.FromResult((IQueryable<TEntity>)dbset);

        public Task<int> SaveChangesAsync() => context.SaveChangesAsync();
    }
}
