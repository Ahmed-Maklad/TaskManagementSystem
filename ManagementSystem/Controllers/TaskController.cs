using Application.Command.Create;
using Application.Command.Delete;
using Application.Command.Update;
using Application.Query;
using Domain.Enum;
using ManagementSystem.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
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

        [Authorize]
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

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create(CreateTaskViewModel model)
        {
            var request = new CreateTaskPostRequest
            {
                Name = model.Name,
                Description = model.Description,
                PriorityType = model.PriorityType,
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

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Update([FromRoute] int id)
        {
            var result = await _mediator.Send(new UpdateTaskGetRequest { TaskId = id });

            if (!result.IsSuccess)
            {
                TempData["Error"] = result.Msg;
                return RedirectToAction("GetAllTasks");
            }

            ViewBag.Users = result.Data?.Users;
            return View(result.Data);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Update(UpdateTaskPostRequest model)
        {

            var result = await _mediator.Send(model);

            if (!result.IsSuccess)
            {
                TempData["Error"] = result.Msg;

                var resultView = await _mediator.Send(new UpdateTaskGetRequest { TaskId = model.Id });
                if (resultView.IsSuccess)
                {
                    ViewBag.Users = resultView.Data?.Users;
                }

                return View(model);
            }

            TempData["Success"] = result.Msg;
            return RedirectToAction("GetAllTasks");
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _mediator.Send(new DeleteTaskRequest { TaskId = id });

            if (!result.IsSuccess)
            {
                TempData["Error"] = result.Msg;
                return RedirectToAction("GetAllTasks");
            }

            TempData["Success"] = result.Msg;
            return RedirectToAction("GetAllTasks");
        }

        public async Task<IActionResult> Details(int id)
        {
            var result = await _mediator.Send(new GetTaskRequest { TaskId = id });

            if (!result.IsSuccess)
            {
                TempData["Error"] = result.Msg;
                return RedirectToAction("GetAllTasks");
            }

            return View(result.Data);
        }

        public async Task<IActionResult> GetAllTasks(Priority? Priority, int? PageNumber, int? PageSize)
        {

            var result = await _mediator.Send(new GetAllTasksRequest
            {
                PriorityType = Priority,
                PageNumber = PageNumber,
                PageSize = PageSize
            });

            ViewData["CurrentPage"] = PageNumber ?? 1;
            ViewData["PageSize"] = PageSize ?? 10;
            ViewData["TotalPages"] = (int)Math.Ceiling((double)result.Data.Count / (PageSize ?? 10));
            ViewData["SelectedPriority"] = Priority?.ToString();

            if (!result.IsSuccess)
            {
                ViewData["Message"] = result.Msg;
            }

            return View(result);
        }

    }
}
