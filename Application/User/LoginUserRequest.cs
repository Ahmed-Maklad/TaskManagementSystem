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
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IValidator<LoginRequest> _validator;

        public LoginHandler(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, IValidator<LoginRequest> validator)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _validator = validator;
        }

        public async Task<ResultView<string>> Handle(LoginRequest request, CancellationToken cancellationToken)
        {
            ResultView<string> resultView = new();

            var validationResult = await _validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                resultView.Msg = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                return resultView;
            }
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                resultView.IsSuccess = false;
                resultView.Data = null;
                resultView.Msg = "Invalid Info Login Email Or Password..!";
                return resultView;
            }

            var signInResult = await _signInManager.PasswordSignInAsync(user.UserName, request.Password, false, false);

            if (!signInResult.Succeeded)
            {
                resultView.IsSuccess = false;
                resultView.Data = null;
                resultView.Msg = "Invalid Info Login Email Or Password..!";
                return resultView;
            }
            resultView.IsSuccess = true;
            resultView.Msg = "Login Successfull";
            return resultView;
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
