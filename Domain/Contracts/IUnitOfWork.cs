namespace Domain.Contracts
{
    public interface IUnitOfWork
    {
        IUserTaskRepository UserTasks { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
