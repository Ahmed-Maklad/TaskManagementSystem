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
        public Periority Priority { get; set; }
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
            ResultView<string> resultView = new();

            var validation = await _validator.ValidateAsync(request, cancellationToken);
            if (!validation.IsValid)
            {
                resultView.IsSuccess = false;
                resultView.Msg = string.Join(" , ", validation.Errors.Select(e => e.ErrorMessage));
                return resultView;
            }

            var taskToUpdate = (await _unitOfWork.UserTasks.GetAllAsync())
                .FirstOrDefault(t => t.Id == request.Id);

            if (taskToUpdate == null)
            {
                resultView.IsSuccess = false;
                resultView.Msg = "Task Not Found.";
                return resultView;
            }

            var currentUserIdStr = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(currentUserIdStr))
            {
                resultView.IsSuccess = false;
                resultView.Msg = "User Is Not Authenticated.";
                return resultView;
            }

            var currentUser = await _userManager.FindByIdAsync(currentUserIdStr);
            if (currentUser == null)
            {
                resultView.IsSuccess = false;
                resultView.Msg = "User Does Not Exist.";
                return resultView;
            }

            var roles = await _userManager.GetRolesAsync(currentUser);
            var isAdmin = roles.Contains("Admin");

            var assignedUserId = isAdmin && int.TryParse(request.AssignedUserId, out int Id) ? Id : currentUser.Id;

            taskToUpdate.Name = request.Name;
            taskToUpdate.Description = request.Description;
            taskToUpdate.Priority = request.Priority;
            taskToUpdate.DueDate = request.DueDate;
            taskToUpdate.AssignedUserId = assignedUserId;

            await _unitOfWork.UserTasks.UpdateAsync(taskToUpdate);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            resultView.IsSuccess = true;
            resultView.Msg = "Task Updated Successfully.";
            resultView.Data = taskToUpdate.Id.ToString();

            return resultView;
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
            RuleFor(x => x.Priority).IsInEnum();
            RuleFor(x => x.AssignedUserId).NotNull().WithMessage("Assigned User is required.");
        }
    }
}
