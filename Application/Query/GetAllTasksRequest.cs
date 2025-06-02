using Application.DTOs;
using Domain.Contracts;
using Domain.Entities;
using Domain.ViewModels;
using Infrastructure.Spesification;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.Query
{
    public class GetAllTasksRequest : IRequest<ResultView<List<GetTasksDTO>>>
    {
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
            ResultView<List<GetTasksDTO>> resultView = new();
            IQueryable<GetTasksDTO> tasks = (await _unitOfWork.UserTasks.GetAllAsync()).Include(t => t.AssignedUser).AsNoTracking()
                .Select(t => new GetTasksDTO
                {
                    Id = t.Id,
                    Name = t.Name,
                    Description = t.Description,
                    Priority = t.Priority,
                    DueDate = t.DueDate,
                    AssignedUserId = t.AssignedUserId.ToString(),
                    AssignedFullName = t.AssignedUser.FullName,
                });

            if (tasks == null)
            {
                resultView.Data = new List<GetTasksDTO>();
                resultView.Msg = "Tasks Not Found Please Create Task";
                resultView.IsSuccess = false;
            }

            resultView.Data = await PaginationSpec.ToPaginateResponseAsync(tasks, request?.PageNumber, request?.PageSize);
            resultView.IsSuccess = true;
            resultView.Msg = "Data Fetched Successfully";
            return resultView;

        }
    }


}
