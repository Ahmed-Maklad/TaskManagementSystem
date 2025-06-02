using Domain.Contracts;
using Persistence.Data;
namespace Infrastructure
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly AppDbContext _context;
        private readonly IUserTaskRepository _userTaskRepository;

        public UnitOfWork(AppDbContext context, IUserTaskRepository userTaskRepository)
        {
            _context = context;
            _userTaskRepository = userTaskRepository;
        }

        public IUserTaskRepository UserTasks => _userTaskRepository;

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken) => _context.SaveChangesAsync(cancellationToken);

        public void Dispose() => _context.Dispose();


    }
}
