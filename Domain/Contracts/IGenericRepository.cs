namespace Domain.Contracts
{
    public interface IGenericRepository<TEntity, TId> where TEntity : class
    {
        public Task<TEntity> CreateAsync(TEntity Entity);
        public Task<TEntity> UpdateAsync(TEntity Entity);

        public Task<TEntity> DeleteAsync(TEntity Entity);
        public Task<IQueryable<TEntity>> GetAllAsync();
        public Task<int> SaveChangesAsync();
    }
}
