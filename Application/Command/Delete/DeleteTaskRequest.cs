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
            var ResultView = new ResultView<string>();

            try
            {
                var Task = (await _unitOfWork.UserTasks.GetAllAsync())
                    .FirstOrDefault(t => t.Id == request.TaskId);

                if (Task == null)
                {
                    ResultView.IsSuccess = false;
                    ResultView.Msg = "Task Not Found.";
                    return ResultView;
                }

                _unitOfWork.UserTasks.DeleteAsync(Task);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                ResultView.IsSuccess = true;
                ResultView.Msg = "Task Deleted Successfully.";
                ResultView.Data = Task.Id.ToString();
            }
            catch (Exception ex)
            {
                ResultView.IsSuccess = false;
                ResultView.Msg = $"An error occurred: {ex.Message}";
            }

            return ResultView;
        }

    }
}
