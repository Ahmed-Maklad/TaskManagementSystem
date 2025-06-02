using Domain.Entities;
using Domain.ViewModels;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.Command.UserCommand
{
    public class RegisterUserRequest : IRequest<ResultView<string>>
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
    public class RegisterUserRequesHandler : IRequestHandler<RegisterUserRequest, ResultView<string>>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IValidator<RegisterUserRequest> _validator;

        public RegisterUserRequesHandler(UserManager<ApplicationUser> userManager, IValidator<RegisterUserRequest> validator)
        {
            _userManager = userManager;
            _validator = validator;
        }

        public async Task<ResultView<string>> Handle(RegisterUserRequest request, CancellationToken cancellationToken)
        {
            ResultView<string> resultView = new();

            var validationResult = await _validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                // If validation fails, join all error messages and add them to Msg
                resultView.Msg = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                return resultView;
            }

            var userName = request.Email.Split('@')[0];

            if (await _userManager.Users.AnyAsync(u => u.UserName == userName || u.Email == request.Email, cancellationToken))
            {
                resultView.IsSuccess = false;
                resultView.Data = null;
                resultView.Msg = "Username or Email already exists.";
                return resultView;
            }

            var user = new ApplicationUser
            {
                FullName = request.FullName,
                Email = request.Email,
                UserName = userName
            };

            var createResult = await _userManager.CreateAsync(user, request.Password);

            if (!createResult.Succeeded)
            {
                resultView.IsSuccess = false;
                resultView.Data = null;
                resultView.Msg = string.Join(", ", createResult.Errors.Select(e => e.Description));
                return resultView;
            }

            await _userManager.AddToRoleAsync(user, "User");

            resultView.IsSuccess = true;
            resultView.Msg = "User registered successfully.";

            return resultView;
        }
    }

    public class RegisterUserRequestValidator : AbstractValidator<RegisterUserRequest>
    {
        public RegisterUserRequestValidator()
        {
            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Full name is required.")
                .MaximumLength(100).WithMessage("Full name can't exceed 100 characters.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters.");
        }
    }

}
