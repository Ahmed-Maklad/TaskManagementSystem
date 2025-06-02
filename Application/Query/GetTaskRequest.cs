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
            var userTask = (await _unitOfWork.UserTasks.GetAllAsync()).FirstOrDefault(P => P.Id == request.TaskId);

            if (userTask == null)
            {
                return new ResultView<GetTasksDTO>
                {
                    IsSuccess = false,
                    Msg = "Task Not Exist.",
                    Data = new GetTasksDTO()
                };
            }

            var taskDto = new GetTasksDTO
            {
                Id = userTask.Id,
                Name = userTask.Name,
                Description = userTask.Description,
                Priority = userTask.Priority,
                DueDate = userTask.DueDate,
                AssignedUserId = userTask.AssignedUserId.ToString(),
                AssignedFullName = userTask.AssignedUser?.FullName
            };

            return new ResultView<GetTasksDTO>
            {
                IsSuccess = true,
                Msg = "Task retrieved successfully.",
                Data = taskDto
            };
        }
    }

}
