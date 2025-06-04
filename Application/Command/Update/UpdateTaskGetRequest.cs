using Domain.Contracts;
using Domain.Entities;
using Domain.ViewModels;
using ManagementSystem.Models;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Application.Command.Update
{
    public class UpdateTaskGetRequest : IRequest<ResultView<UpdateTaskViewModel>>
    {
        public int TaskId { get; set; }
    }
    public class UpdateTaskQueryRequestHandler : IRequestHandler<UpdateTaskGetRequest, ResultView<UpdateTaskViewModel>>
    {
        private readonly IUnitOfWork _UnitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UpdateTaskQueryRequestHandler(IUnitOfWork UnitOfWork, UserManager<ApplicationUser> userManager,
            IHttpContextAccessor httpContextAccessor)
        {
            _UnitOfWork = UnitOfWork;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ResultView<UpdateTaskViewModel>> Handle(UpdateTaskGetRequest request, CancellationToken cancellationToken)
        {
            ResultView<UpdateTaskViewModel> ResultView = new();

            try
            {
                var TaskBySearch = (await _UnitOfWork.UserTasks.GetAllAsync()).FirstOrDefault(P => P.Id == request.TaskId);
                if (TaskBySearch is null)
                {
                    ResultView.Data = null;
                    ResultView.IsSuccess = false;
                    ResultView.Msg = "Task Not Exist Please Create This First";
                    return ResultView;
                }

                var httpUser = _httpContextAccessor.HttpContext?.User;
                var isAdmin = httpUser != null && httpUser.IsInRole("Admin");

                ResultView.IsSuccess = true;
                ResultView.Msg = "Task Exist True";

                ResultView.Data = new UpdateTaskViewModel
                {
                    Id = TaskBySearch.Id,
                    Name = TaskBySearch.Name,
                    Description = TaskBySearch.Description,
                    PriorityType = TaskBySearch.PriorityType,
                    DueDate = TaskBySearch.DueDate,
                    AssignedUserId = TaskBySearch.AssignedUserId.ToString(),
                    Users = isAdmin ? await _userManager.Users.Select(u => new SelectListItem
                    {
                        Value = u.Id.ToString(),
                        Text = u.UserName
                    }).ToListAsync(cancellationToken) : null
                };
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
