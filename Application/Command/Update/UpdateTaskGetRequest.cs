using Domain.Contracts;
using Domain.Entities;
using Domain.ViewModels;
using ManagementSystem.Models;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
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
            ResultView<UpdateTaskViewModel> resultView = new();
            var TaskBySearch = (await _UnitOfWork.UserTasks.GetAllAsync()).FirstOrDefault(P => P.Id == request.TaskId);
            if (TaskBySearch is null)
            {
                resultView.Data = null;
                resultView.IsSuccess = false;
                resultView.Msg = "Task Not Exist Please Create This First";
            }
            var httpUser = _httpContextAccessor.HttpContext?.User;
            var isAdmin = httpUser != null && httpUser.IsInRole("Admin");

            resultView.IsSuccess = true;
            resultView.Msg = "Task Exist True";

            resultView.Data = new UpdateTaskViewModel
            {
                Id = TaskBySearch.Id,
                Name = TaskBySearch.Name,
                Description = TaskBySearch.Description,
                Priority = TaskBySearch.Priority,
                DueDate = TaskBySearch.DueDate,
                AssignedUserId = TaskBySearch.AssignedUserId.ToString(),
                Users = isAdmin ? await _userManager.Users.Select(u => new SelectListItem
                {
                    Value = u.Id.ToString(),
                    Text = u.UserName
                }).ToListAsync(cancellationToken) : null
            };

            return resultView;
        }
    }
}
