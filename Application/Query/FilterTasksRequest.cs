using Application.DTOs;
using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using Domain.Contracts;
using Domain.Entities;
using Domain.Enum;
using Domain.ViewModels;
using Infrastructure.Spesification;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Query
{
    public class FilterTasksRequest : IRequest<ResultView<List<GetTasksDTO>>>
    {
        public Priority? PeriorityType { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }

    public class FilterTaskRequestHandler : IRequestHandler<FilterTasksRequest, ResultView<List<GetTasksDTO>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public FilterTaskRequestHandler(IUnitOfWork unitOfWork)
            => _unitOfWork = unitOfWork;

        public async Task<ResultView<List<GetTasksDTO>>> Handle(FilterTasksRequest request, CancellationToken cancellationToken)
        {
            var ResultView = new ResultView<List<GetTasksDTO>>();

            try
            {
                if (request == null)
                {
                    ResultView.IsSuccess = false;
                    ResultView.Msg = "Please Select Periority Level From Drop List";
                    ResultView.Data = null;
                    return ResultView;
                }

                var spec = new FilterTasksWithIncludesSpecification(request.PeriorityType.Value);

                var TasksUserFilter = (await _unitOfWork.UserTasks.GetAllAsync()).AsTracking();

                var tasks = SpecificationEvaluator.Default.GetQuery(TasksUserFilter, spec);

                if (tasks == null || !tasks.Any())
                {
                    ResultView.IsSuccess = false;
                    ResultView.Msg = "No Tasks Found With Selected Filters.";
                    ResultView.Data = new List<GetTasksDTO>();
                    return ResultView;
                }

                ResultView.Data = (await PaginationSpec.ToPaginateResponseAsync(tasks, request?.PageNumber, request?.PageSize))
                    .Select(t => new GetTasksDTO
                    {
                        Id = t.Id,
                        Name = t.Name,
                        Description = t.Description,
                        PriorityType = t.PriorityType,
                        DueDate = t.DueDate,
                        AssignedUserId = t.AssignedUserId.ToString(),
                        AssignedFullName = t.AssignedUser.FullName,
                    }).ToList();

                ResultView.IsSuccess = true;
                ResultView.Msg = "Tasks fetched successfully.";
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

    public class FilterTasksWithIncludesSpecification : Specification<UserTask>
    {
        public FilterTasksWithIncludesSpecification(Priority priority)
        {
            Query.Where(task => task.PriorityType == priority);
            Query.Include(task => task.AssignedUser);
        }
    }
}
