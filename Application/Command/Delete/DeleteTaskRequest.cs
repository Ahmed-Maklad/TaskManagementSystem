using Domain.Contracts;
using Domain.ViewModels;
using MediatR;

namespace Application.Command.Delete
{
    public class DeleteTaskRequest : IRequest<ResultView<string>>
    {
        public int TaskId { get; set; }
    }
    public class DeleteTaskRequestHandler : IRequestHandler<DeleteTaskRequest, ResultView<string>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteTaskRequestHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ResultView<string>> Handle(DeleteTaskRequest request, CancellationToken cancellationToken)
        {
            var result = new ResultView<string>();

            var task = (await _unitOfWork.UserTasks.GetAllAsync())
                .FirstOrDefault(t => t.Id == request.TaskId);

            if (task == null)
            {
                result.IsSuccess = false;
                result.Msg = "Task Not Found.";
                return result;
            }

            _unitOfWork.UserTasks.DeleteAsync(task);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            result.IsSuccess = true;
            result.Msg = "Task Deleted Successfully.";
            result.Data = task.Id.ToString();

            return result;
        }
    }
}
