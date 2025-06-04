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
            ResultView<string> ResultView = new();

            try
            {
                var ValidationResult = await _validator.ValidateAsync(request, cancellationToken);

                if (!ValidationResult.IsValid)
                {
                    ResultView.Msg = string.Join(", ", ValidationResult.Errors.Select(e => e.ErrorMessage));
                    return ResultView;
                }

                var UserName = request.Email.Split('@')[0];

                if (await _userManager.Users.AnyAsync(u => u.UserName == UserName || u.Email == request.Email, cancellationToken))
                {
                    ResultView.IsSuccess = false;
                    ResultView.Data = null;
                    ResultView.Msg = "Username or Email already exists.";
                    return ResultView;
                }

                var user = new ApplicationUser
                {
                    FullName = request.FullName,
                    Email = request.Email,
                    UserName = UserName
                };

                var createResult = await _userManager.CreateAsync(user, request.Password);

                if (!createResult.Succeeded)
                {
                    ResultView.IsSuccess = false;
                    ResultView.Data = null;
                    ResultView.Msg = string.Join(", ", createResult.Errors.Select(e => e.Description));
                    return ResultView;
                }

                await _userManager.AddToRoleAsync(user, "User");

                ResultView.IsSuccess = true;
                ResultView.Msg = "User registered successfully.";
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
