using Application.DTOs;
using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using Domain.Contracts;
using Domain.Entities;
using Domain.Enum;
using Domain.ViewModels;
using MediatR;

namespace Application.Query
{
    public class FilterTasksRequest : IRequest<ResultView<List<GetTasksDTO>>>
    {
        public Periority? PeriorityType { get; set; }
    }
    public class FilterTaskRequestHandler : IRequestHandler<FilterTasksRequest, ResultView<List<GetTasksDTO>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public FilterTaskRequestHandler(IUnitOfWork unitOfWork)
            => _unitOfWork = unitOfWork;

        public async Task<ResultView<List<GetTasksDTO>>> Handle(FilterTasksRequest request, CancellationToken cancellationToken)
        {
            var resultView = new ResultView<List<GetTasksDTO>>();

            if (request == null)
            {
                resultView.IsSuccess = false;
                resultView.Msg = "Please Select Periority Level From Drop List";
                resultView.Data = null;
                return resultView;
            }

            var spec = new FilterTasksWithIncludesSpecification(request.PeriorityType.Value);

            var TasksUserFilter = await _unitOfWork.UserTasks.GetAllAsync();

            var tasks = SpecificationEvaluator.Default.GetQuery(TasksUserFilter, spec);

            if (tasks == null || !tasks.Any())
            {
                resultView.IsSuccess = false;
                resultView.Msg = "No Tasks Found With Selected Filters.";
                resultView.Data = new List<GetTasksDTO>();
                return resultView;
            }

            resultView.Data = tasks.Select(task => new GetTasksDTO
            {
                Id = task.Id,
                Name = task.Name,
                Description = task.Description,
                Priority = task.Priority,
                AssignedFullName = task.AssignedUser.FullName,
                DueDate = task.DueDate,
                AssignedUserId = task.AssignedUserId.ToString()
            }).ToList();

            resultView.IsSuccess = true;
            resultView.Msg = "Tasks fetched successfully.";
            return resultView;
        }
    }
    public class FilterTasksWithIncludesSpecification : Specification<UserTask>
    {
        public FilterTasksWithIncludesSpecification(Periority priority)
        {
            Query.Where(task => task.Priority == priority);
            Query.Include(task => task.AssignedUser);
        }
    }
}
