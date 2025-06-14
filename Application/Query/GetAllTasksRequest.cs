using Application.DTOs;
using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using Domain.Contracts;
using Domain.Entities;
using Domain.Enum;
using Domain.ViewModels;
using Infrastructure.Spesification;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.Query
{
    public class GetAllTasksRequest : IRequest<ResultView<List<GetTasksDTO>>>
    {
        public Priority? PriorityType { get; set; }
        public int? PageNumber { get; set; }
        public int? PageSize { get; set; }
    }
    public class GetAllTasksHandler : IRequestHandler<GetAllTasksRequest, ResultView<List<GetTasksDTO>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;

        public GetAllTasksHandler(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public async Task<ResultView<List<GetTasksDTO>>> Handle(GetAllTasksRequest request, CancellationToken cancellationToken)
        {
            ResultView<List<GetTasksDTO>> ResultView = new();

            try
            {
                var spec = new FilterTasksWithIncludesSpecification(request.PriorityType);
                var userTasksQueryable = (await _unitOfWork.UserTasks.GetAllAsync()).AsNoTracking();
                var filteredTasks = SpecificationEvaluator.Default.GetQuery(userTasksQueryable, spec);

                var data = await PaginationSpec.ToPaginateResponseAsync(filteredTasks, request.PageNumber, request.PageSize);
                if (data.Count > 0 || data is not null)
                {
                    ResultView.Data = data;
                    ResultView.IsSuccess = true;
                    ResultView.Msg = "Data Fetched Successfully";
                }
                else
                {
                    ResultView.Data = null;
                    ResultView.IsSuccess = false;
                    ResultView.Msg = "Data Fetched Failed";
                }
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

    public class FilterTasksWithIncludesSpecification : Specification<UserTask, GetTasksDTO>
    {
        public FilterTasksWithIncludesSpecification(Priority? priority)
        {
            if (priority.HasValue)
            {
                Query.Where(task => task.PriorityType == priority.Value);
            }

            Query.Select(task => new GetTasksDTO
            {
                Id = task.Id,
                Name = task.Name,
                Description = task.Description,
                PriorityType = task.PriorityType,
                DueDate = task.DueDate,
                AssignedUserId = task.AssignedUserId.ToString(),
                AssignedUserFullName = task.AssignedUser != null ? task.AssignedUser.FullName : null,
            });
        }
    }
}
