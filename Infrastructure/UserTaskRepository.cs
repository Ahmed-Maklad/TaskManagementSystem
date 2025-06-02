using Domain.Contracts;
using Domain.Entities;
using Persistence.Data;
namespace Infrastructure
{
    public class UserTaskRepository : GenericRepository<UserTask, int>, IUserTaskRepository
    {
        public UserTaskRepository(AppDbContext context) : base(context)
        {

        }
    }
}
