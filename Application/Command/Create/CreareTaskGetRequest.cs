using System.Security.Claims;
using Domain.Entities;
using Domain.ViewModels;
using ManagementSystem.Models;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.Command.Create
{
    public class CreareTaskGetRequest : IRequest<ResultView<CreateTaskViewModel>>
    {

    }
    public class CreateTaskCommandRequestHandler : IRequestHandler<CreareTaskGetRequest, ResultView<CreateTaskViewModel>>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CreateTaskCommandRequestHandler(UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ResultView<CreateTaskViewModel>> Handle(CreareTaskGetRequest request, CancellationToken cancellationToken)
        {
            ResultView<CreateTaskViewModel> resultView = new();
            var currentUserId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(currentUserId))
            {
                resultView.IsSuccess = false;
                resultView.Data = null;
                resultView.Msg = "User Not Authinticated";
                return resultView;
            }

            var currentUser = await _userManager.FindByIdAsync(currentUserId);

            if (currentUser == null)
            {
                resultView.IsSuccess = false;
                resultView.Data = null;
                resultView.Msg = "User Not Exist";
                return resultView;
            }

            var roles = await _userManager.GetRolesAsync(currentUser);
            var IsAdmin = roles.Contains("Admin");
            resultView.IsSuccess = true;
            resultView.Msg = "User Exist True..";
            resultView.Data = new CreateTaskViewModel
            {
                DueDate = DateTime.Today.AddDays(1),
                IsAdmin = IsAdmin,
                AssignedUserId = IsAdmin ? null : currentUserId,
                Users = IsAdmin ? await _userManager.Users.Select(u => new SelectListItem
                {
                    Value = u.Id.ToString(),
                    Text = u.UserName
                }).ToListAsync() : new List<SelectListItem>()
            };

            return resultView;
        }
    }

}
