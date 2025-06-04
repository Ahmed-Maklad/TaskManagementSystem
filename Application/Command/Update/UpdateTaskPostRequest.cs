using System.Security.Claims;
using Domain.Contracts;
using Domain.Entities;
using Domain.Enum;
using Domain.ViewModels;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Application.Command.Update
{
    public class UpdateTaskPostRequest : IRequest<ResultView<string>>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Priority PriorityType { get; set; }
        public DateTime DueDate { get; set; }
        public string AssignedUserId { get; set; }
    }
    public class UpdateTaskPostRequestHandler : IRequestHandler<UpdateTaskPostRequest, ResultView<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IValidator<UpdateTaskPostRequest> _validator;

        public UpdateTaskPostRequestHandler(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager,
                                            IHttpContextAccessor httpContextAccessor, IValidator<UpdateTaskPostRequest> validator)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _validator = validator;
        }

        public async Task<ResultView<string>> Handle(UpdateTaskPostRequest request, CancellationToken cancellationToken)
        {
            ResultView<string> ResultView = new();

            try
            {
                var Validation = await _validator.ValidateAsync(request, cancellationToken);
                if (!Validation.IsValid)
                {
                    ResultView.IsSuccess = false;
                    ResultView.Msg = string.Join(" , ", Validation.Errors.Select(e => e.ErrorMessage));
                    return ResultView;
                }

                var TaskToUpdate = (await _unitOfWork.UserTasks.GetAllAsync())
                    .FirstOrDefault(t => t.Id == request.Id);

                if (TaskToUpdate == null)
                {
                    ResultView.IsSuccess = false;
                    ResultView.Msg = "Task Not Found.";
                    return ResultView;
                }

                var CurrentUserIdStr = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(CurrentUserIdStr))
                {
                    ResultView.IsSuccess = false;
                    ResultView.Msg = "User Is Not Authenticated.";
                    return ResultView;
                }

                var CurrentUser = await _userManager.FindByIdAsync(CurrentUserIdStr);
                if (CurrentUser == null)
                {
                    ResultView.IsSuccess = false;
                    ResultView.Msg = "User Does Not Exist.";
                    return ResultView;
                }

                var roles = await _userManager.GetRolesAsync(CurrentUser);
                var isAdmin = roles.Contains("Admin");

                var assignedUserId = isAdmin && int.TryParse(request.AssignedUserId, out int Id) ? Id : CurrentUser.Id;

                TaskToUpdate.Name = request.Name;
                TaskToUpdate.Description = request.Description;
                TaskToUpdate.PriorityType = request.PriorityType;
                TaskToUpdate.DueDate = request.DueDate;
                TaskToUpdate.AssignedUserId = assignedUserId;

                await _unitOfWork.UserTasks.UpdateAsync(TaskToUpdate);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                ResultView.IsSuccess = true;
                ResultView.Msg = "Task Updated Successfully.";
                ResultView.Data = TaskToUpdate.Id.ToString();
            }
            catch (Exception ex)
            {
                ResultView.IsSuccess = false;
                ResultView.Msg = $"An error occurred: {ex.Message}";
            }

            return ResultView;
        }

    }
    public class UpdateTaskPostRequestValidator : AbstractValidator<UpdateTaskPostRequest>
    {
        public UpdateTaskPostRequestValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0).WithMessage("Invalid Task ID.");
            RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Description).MaximumLength(1000);
            RuleFor(x => x.DueDate).GreaterThanOrEqualTo(DateTime.Today);
            RuleFor(x => x.PriorityType).IsInEnum();
            RuleFor(x => x.AssignedUserId).NotNull().WithMessage("Assigned User is required.");
        }
    }
}
