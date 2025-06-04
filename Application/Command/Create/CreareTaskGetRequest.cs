using System.Security.Claims;
using Domain.Entities;
using Domain.ViewModels;
using ManagementSystem.Models;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
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
            ResultView<CreateTaskViewModel> ResultView = new();

            try
            {
                var CurrentUserId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(CurrentUserId))
                {
                    ResultView.IsSuccess = false;
                    ResultView.Data = null;
                    ResultView.Msg = "User Not Authenticated";
                    return ResultView;
                }

                var CurrentUser = await _userManager.FindByIdAsync(CurrentUserId);

                if (CurrentUser == null)
                {
                    ResultView.IsSuccess = false;
                    ResultView.Data = null;
                    ResultView.Msg = "User Not Exist";
                    return ResultView;
                }

                var roles = await _userManager.GetRolesAsync(CurrentUser);
                var isAdmin = roles.Contains("Admin");

                ResultView.IsSuccess = true;
                ResultView.Msg = "User Exists True..";
                ResultView.Data = new CreateTaskViewModel
                {
                    DueDate = DateTime.Today.AddDays(1),
                    IsAdmin = isAdmin,
                    AssignedUserId = isAdmin ? null : CurrentUserId,
                    Users = isAdmin ? await _userManager.Users.Select(u => new SelectListItem
                    {
                        Value = u.Id.ToString(),
                        Text = u.UserName
                    }).ToListAsync() : new List<SelectListItem>()
                };
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

}
