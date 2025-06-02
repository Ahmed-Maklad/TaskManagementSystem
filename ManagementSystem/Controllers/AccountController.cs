using Application.Command.UserCommand;
using Application.User;
using MediatR;
using Microsoft.AspNetCore.Mvc;



namespace ManagementSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private readonly IMediator _mediator;
        public AccountController(ILogger<AccountController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterUserRequest request)
        {

            var ResultView = await _mediator.Send(request);

            if (!ResultView.IsSuccess)
            {
                ModelState.AddModelError(string.Empty, ResultView.Msg);
                return View(request);
            }

            TempData["SuccessMessage"] = ResultView.Msg;
            return RedirectToAction("Login", "Account");
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var ResultView = await _mediator.Send(request);

            if (!ResultView.IsSuccess)
            {
                ModelState.AddModelError(string.Empty, ResultView.Msg);
                return View(request);
            }

            TempData["SuccessMessage"] = ResultView.Msg;
            return RedirectToAction("GetAllTasks", "Task");
        }

    }
}
