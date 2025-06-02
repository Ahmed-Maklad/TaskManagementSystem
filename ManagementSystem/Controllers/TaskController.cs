using Application.Command.Create;
using Application.Query;
using ManagementSystem.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ManagementSystem.Controllers
{
    public class TaskController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private readonly IMediator _mediator;
        public TaskController(ILogger<AccountController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var result = await _mediator.Send(new CreareTaskGetRequest());

            if (!result.IsSuccess)
            {
                ViewData["Message"] = result.Msg;
                return RedirectToAction("GetAllTasks");
            }

            return View(result.Data);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateTaskViewModel model)
        {
            var request = new CreateTaskPostRequest
            {
                Name = model.Name,
                Description = model.Description,
                Priority = model.Priority,
                DueDate = model.DueDate,
                AssignedUserId = model.AssignedUserId
            };

            var result = await _mediator.Send(request);

            if (!result.IsSuccess)
            {
                ViewData["Message"] = result.Msg;

                var getData = await _mediator.Send(new CreareTaskGetRequest());

                return View(getData.Data);
            }

            TempData["SuccessMessage"] = "Task created successfully.";
            return RedirectToAction("GetAllTasks");
        }



        public async Task<IActionResult> GetAllTasks(int? PageNumber, int? PageSize)
        {

            var result = await _mediator.Send(new GetAllTasksRequest
            {
                PageNumber = PageNumber,
                PageSize = PageSize
            });

            ViewData["CurrentPage"] = PageNumber ?? 1;
            ViewData["PageSize"] = PageSize ?? 10;
            ViewData["TotalPages"] = (int)Math.Ceiling((double)result.Data.Count / (PageSize ?? 10));

            if (!result.IsSuccess)
            {
                ViewData["Message"] = result.Msg;
            }

            return View(result);
        }

    }
}
