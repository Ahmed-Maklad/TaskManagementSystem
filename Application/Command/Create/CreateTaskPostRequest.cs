using System.Security.Claims;
using Domain.Contracts;
using Domain.Entities;
using Domain.Enum;
using Domain.ViewModels;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Application.Command.Create
{
    public class CreateTaskPostRequest : IRequest<ResultView<string>>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Priority PriorityType { get; set; }
        public DateTime DueDate { get; set; }
        public string AssignedUserId { get; set; }
    }
    public class CreateTaskPostRequestHandler : IRequestHandler<CreateTaskPostRequest, ResultView<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IValidator<CreateTaskPostRequest> _validator;

        public CreateTaskPostRequestHandler(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager,
            IHttpContextAccessor httpContextAccessor, IValidator<CreateTaskPostRequest> validator)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _validator = validator;
        }

        public async Task<ResultView<string>> Handle(CreateTaskPostRequest request, CancellationToken cancellationToken)
        {
            ResultView<string> ResultView = new();

            try
            {
                var ValidationResult = await _validator.ValidateAsync(request, cancellationToken);
                if (!ValidationResult.IsValid)
                {
                    ResultView.Msg = string.Join(", ", ValidationResult.Errors.Select(e => e.ErrorMessage));
                    return ResultView;
                }

                var CurrentUserIdStr = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(CurrentUserIdStr))
                {
                    ResultView.IsSuccess = false;
                    ResultView.Msg = "User is not authenticated.";
                    return ResultView;
                }

                var CurrentUser = await _userManager.FindByIdAsync(CurrentUserIdStr);
                if (CurrentUser == null)
                {
                    ResultView.IsSuccess = false;
                    ResultView.Msg = "User not Exist.";
                    return ResultView;
                }

                var task = new UserTask
                {
                    Name = request.Name,
                    Description = request.Description,
                    DueDate = request.DueDate,
                    PriorityType = request.PriorityType,
                    AssignedUserId = int.TryParse(request.AssignedUserId, out int Id) ? Id : CurrentUser.Id
                };

                await _unitOfWork.UserTasks.CreateAsync(task);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                ResultView.IsSuccess = true;
                ResultView.Msg = "Task created successfully.";
                ResultView.Data = task.Id.ToString();
            }
            catch (Exception ex)
            {
                ResultView.IsSuccess = false;
                ResultView.Msg = $"An error occurred: {ex.Message}";
            }

            return ResultView;
        }
    }

    public class CreateTaskPostRequestValidator : AbstractValidator<CreateTaskPostRequest>
    {
        public CreateTaskPostRequestValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Description).MaximumLength(1000);
            RuleFor(x => x.DueDate).GreaterThanOrEqualTo(DateTime.Today);
            RuleFor(x => x.PriorityType).IsInEnum();
            RuleFor(x => x.AssignedUserId).NotNull().WithMessage("Assigned User is required.");
        }
    }
}
