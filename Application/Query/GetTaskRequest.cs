using Application.DTOs;
using Domain.Contracts;
using Domain.ViewModels;
using MediatR;

namespace Application.Query
{
    public class GetTaskRequest : IRequest<ResultView<GetTasksDTO>>
    {
        public int TaskId { get; set; }
    }
    public class GetTaskRequestHandler : IRequestHandler<GetTaskRequest, ResultView<GetTasksDTO>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetTaskRequestHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ResultView<GetTasksDTO>> Handle(GetTaskRequest request, CancellationToken cancellationToken)
        {
            ResultView<GetTasksDTO> ResultView = new();

            try
            {
                var UserTask = (await _unitOfWork.UserTasks.GetAllAsync()).FirstOrDefault(P => P.Id == request.TaskId);

                if (UserTask == null)
                {
                    ResultView.IsSuccess = false;
                    ResultView.Msg = "Task Not Exist.";
                    ResultView.Data = new GetTasksDTO();
                    return ResultView;
                }

                var TaskDto = new GetTasksDTO
                {
                    Id = UserTask.Id,
                    Name = UserTask.Name,
                    Description = UserTask.Description,
                    PriorityType = UserTask.PriorityType,
                    DueDate = UserTask.DueDate,
                    AssignedUserId = UserTask.AssignedUserId.ToString(),
                    AssignedFullName = UserTask.AssignedUser?.FullName
                };

                ResultView.IsSuccess = true;
                ResultView.Msg = "Task retrieved successfully.";
                ResultView.Data = TaskDto;
            }
            catch (Exception ex)
            {
                ResultView.IsSuccess = false;
                ResultView.Msg = $"An error occurred: {ex.Message}";
                ResultView.Data = null;
            }

            return ResultView;
        }

    }

}
