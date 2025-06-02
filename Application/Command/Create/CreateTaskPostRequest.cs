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
        public Periority Priority { get; set; }
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
            ResultView<string> resultView = new();

            var validationResult = await _validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                resultView.Msg = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                return resultView;
            }

            var currentUserIdStr = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(currentUserIdStr))
            {
                resultView.IsSuccess = false;
                resultView.Msg = "User is not authenticated.";
                return resultView;
            }

            var currentUser = await _userManager.FindByIdAsync(currentUserIdStr);
            if (currentUser == null)
            {
                resultView.IsSuccess = false;
                resultView.Msg = "User not Exist.";
                return resultView;
            }

            var task = new UserTask
            {
                Name = request.Name,
                Description = request.Description,
                DueDate = request.DueDate,
                Priority = request.Priority,
                AssignedUserId = int.TryParse(request.AssignedUserId, out int Id) ? Id : currentUser.Id
            };

            await _unitOfWork.UserTasks.CreateAsync(task);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            resultView.IsSuccess = true;
            resultView.Msg = "Task created successfully.";
            resultView.Data = task.Id.ToString();
            return resultView;
        }
    }
    public class CreateTaskPostRequestValidator : AbstractValidator<CreateTaskPostRequest>
    {
        public CreateTaskPostRequestValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Description).MaximumLength(1000);
            RuleFor(x => x.DueDate).GreaterThanOrEqualTo(DateTime.Today);
            RuleFor(x => x.Priority).IsInEnum();
            RuleFor(x => x.AssignedUserId).NotNull().WithMessage("Assigned User is required.");
        }
    }
}
