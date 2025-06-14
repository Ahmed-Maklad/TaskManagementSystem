using Application.DTOs;
using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using Domain.Contracts;
using Domain.Entities;
using Domain.ViewModels;
using MediatR;
using Microsoft.EntityFrameworkCore;

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
                var spec = new TaskByIdWithIncludesSpecification(request.TaskId);

                var userTasksIQuarable = (await _unitOfWork.UserTasks.GetAllAsync()).AsNoTracking();
                var task = await SpecificationEvaluator.Default.GetQuery(userTasksIQuarable, spec)
                    .FirstOrDefaultAsync(cancellationToken);

                ResultView.IsSuccess = true;
                ResultView.Msg = "Task retrieved successfully.";
                ResultView.Data = task;
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

    public class TaskByIdWithIncludesSpecification : Specification<UserTask, GetTasksDTO>
    {
        public TaskByIdWithIncludesSpecification(int taskId)
        {
            Query.Where(task => task.Id == taskId);

            Query.Select(task => new GetTasksDTO
            {
                Id = task.Id,
                Name = task.Name,
                Description = task.Description,
                PriorityType = task.PriorityType,
                DueDate = task.DueDate,
                AssignedUserId = task.AssignedUserId.ToString(),
                AssignedUserFullName = task.AssignedUser != null ? task.AssignedUser.FullName : null
            });
        }
    }

}
