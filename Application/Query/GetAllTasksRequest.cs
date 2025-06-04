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
                IQueryable<GetTasksDTO> Tasks = (await _unitOfWork.UserTasks.GetAllAsync())
                    .Include(t => t.AssignedUser)
                    .AsNoTracking()
                    .Select(t => new GetTasksDTO
                    {
                        Id = t.Id,
                        Name = t.Name,
                        Description = t.Description,
                        PriorityType = t.PriorityType,
                        DueDate = t.DueDate,
                        AssignedUserId = t.AssignedUserId.ToString(),
                        AssignedFullName = t.AssignedUser.FullName,
                    });

                if (request.PriorityType.HasValue)
                {
                    var spec = new FilterTasksWithIncludesSpecification(request.PriorityType.Value);
                    var TasksUserFilter = (await _unitOfWork.UserTasks.GetAllAsync()).AsNoTracking();
                    Tasks = SpecificationEvaluator.Default.GetQuery(TasksUserFilter, spec).Select(t => new GetTasksDTO
                    {
                        Id = t.Id,
                        Name = t.Name,
                        Description = t.Description,
                        PriorityType = t.PriorityType,
                        DueDate = t.DueDate,
                        AssignedUserId = t.AssignedUserId.ToString(),
                        AssignedFullName = t.AssignedUser.FullName,
                    });
                }

                ResultView.Data = await PaginationSpec.ToPaginateResponseAsync(Tasks, request.PageNumber, request.PageSize);
                ResultView.IsSuccess = true;
                ResultView.Msg = "Data Fetched Successfully";
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
