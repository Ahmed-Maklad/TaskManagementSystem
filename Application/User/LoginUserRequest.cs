using Domain.Entities;
using Domain.ViewModels;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.User
{
    public class LoginRequest : IRequest<ResultView<string>>
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
    public class LoginHandler : IRequestHandler<LoginRequest, ResultView<string>>
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _UserManager;
        private readonly IValidator<LoginRequest> _validator;

        public LoginHandler(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> UserManager, IValidator<LoginRequest> validator)
        {
            _signInManager = signInManager;
            _UserManager = UserManager;
            _validator = validator;
        }

        public async Task<ResultView<string>> Handle(LoginRequest request, CancellationToken cancellationToken)
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

                var User = await _UserManager.FindByEmailAsync(request.Email);
                if (User == null)
                {
                    ResultView.IsSuccess = false;
                    ResultView.Data = null;
                    ResultView.Msg = "Invalid Info Login Email Or Password..!";
                    return ResultView;
                }

                var SignInResult = await _signInManager.PasswordSignInAsync(User.UserName, request.Password, false, false);

                if (!SignInResult.Succeeded)
                {
                    ResultView.IsSuccess = false;
                    ResultView.Data = null;
                    ResultView.Msg = "Invalid Info Login Email Or Password..!";
                    return ResultView;
                }

                ResultView.IsSuccess = true;
                ResultView.Msg = "Login Successfull";
            }
            catch (Exception ex)
            {
                ResultView.IsSuccess = false;
                ResultView.Msg = $"An error occurred: {ex.Message}";
                ResultView.Data = null;
            }

            return ResultView;
        }

        public class LoginRequestValidator : AbstractValidator<LoginRequest>
        {
            public LoginRequestValidator()
            {
                RuleFor(x => x.Email)
                    .NotEmpty().WithMessage("Email is required.")
                    .EmailAddress().WithMessage("Invalid email format.");

                RuleFor(x => x.Password)
                    .NotEmpty().WithMessage("Password is required.")
                    .MinimumLength(6).WithMessage("Password must be at least 6 characters.");
            }
        }
    }
}
