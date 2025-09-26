using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication7.Models;
using TaskStatus = WebApplication7.Models.TaskStatus;
using System.Security.Claims;

namespace ToDoList.Controllers
{
    [Authorize]
    public class TasksController : Controller
    {
        private readonly ToDoListContext _context;
        private readonly UserManager<User> _userManager;

        public TasksController(ToDoListContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        private bool IsAdmin() => User.IsInRole("admin");

        public IActionResult Index(TaskFilter filter)
        {
            var tasks = _context.ToDoTasks
                .Include(t => t.Creator)
                .Include(t => t.Executor)
                .AsQueryable();

            if (filter == null) filter = new TaskFilter();

            if (!string.IsNullOrEmpty(filter.Title))
                tasks = tasks.Where(t => t.Title.Contains(filter.Title));

            if (filter.CreatedFrom.HasValue)
                tasks = tasks.Where(t => t.CreatedAt >= filter.CreatedFrom);

            if (filter.CreatedTo.HasValue)
                tasks = tasks.Where(t => t.CreatedAt <= filter.CreatedTo);

            if (!string.IsNullOrEmpty(filter.Keywords))
                tasks = tasks.Where(t => t.Description.Contains(filter.Keywords));

            if (filter.Priority.HasValue)
                tasks = tasks.Where(t => t.Priority == filter.Priority);

            if (filter.Status.HasValue)
                tasks = tasks.Where(t => t.Status == filter.Status);

            tasks = filter.SortOrder switch
            {
                "Title" => filter.Descending ? tasks.OrderByDescending(t => t.Title) : tasks.OrderBy(t => t.Title),
                "Priority" => filter.Descending ? tasks.OrderByDescending(t => t.Priority) : tasks.OrderBy(t => t.Priority),
                "Status" => filter.Descending ? tasks.OrderByDescending(t => t.Status) : tasks.OrderBy(t => t.Status),
                "CreatedAt" => filter.Descending ? tasks.OrderByDescending(t => t.CreatedAt) : tasks.OrderBy(t => t.CreatedAt),
                _ => tasks.OrderBy(t => t.Id)
            };

            return View(tasks.ToList());
        }

        public async Task<IActionResult> Details(int id)
        {
            var task = await _context.ToDoTasks
                .Include(t => t.Creator)
                .Include(t => t.Executor)
                .FirstOrDefaultAsync(t => t.Id == id);
            if (task == null) return NotFound();

            return View(task);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ToDoTask task)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Challenge();

            task.CreatorId = user.Id;
            task.ExecutorId = null;
            task.Status = TaskStatus.Новая;
            task.CreatedAt = DateTime.UtcNow;

            if (!ModelState.IsValid)
            {
                return View(task);
            }

            _context.ToDoTasks.Add(task);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Задача создана";
            return RedirectToAction(nameof(Index));
        }

        
        public async Task<IActionResult> Open(int id)
        {
            var task = await _context.ToDoTasks.FindAsync(id);
            if (task == null) return NotFound();

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return Challenge();

            if (task.ExecutorId != currentUser.Id && !IsAdmin())
            {
                TempData["ErrorMessage"] = "Открыть задачу может только назначенный исполнитель.";
                return RedirectToAction(nameof(Details), new { id });
            }

            if (task.Status == TaskStatus.Новая)
            {
                task.Status = TaskStatus.Открыта;
                task.OpenedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Задача открыта";
            }

            return RedirectToAction(nameof(Details), new { id });
        }
        
        public async Task<IActionResult> Close(int id)
        {
            var task = await _context.ToDoTasks.FindAsync(id);
            if (task == null) return NotFound();

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return Challenge();

            if (task.ExecutorId != currentUser.Id && !IsAdmin())
            {
                TempData["ErrorMessage"] = "Закрыть задачу может только назначенный исполнитель.";
                return RedirectToAction(nameof(Details), new { id });
            }

            if (task.Status == TaskStatus.Открыта)
            {
                task.Status = TaskStatus.Закрыта;
                task.ClosedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Задача закрыта";
            }

            return RedirectToAction(nameof(Details), new { id });
        }
        
        public async Task<IActionResult> Edit(int id)
        {
            var task = await _context.ToDoTasks.FindAsync(id);
            if (task == null) return NotFound();

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return Challenge();

            if (task.CreatorId != currentUser.Id && !IsAdmin())
            {
                TempData["ErrorMessage"] = "Редактировать задачу может только её создатель.";
                return RedirectToAction(nameof(Details), new { id });
            }

            return View(task);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ToDoTask updatedTask)
        {
            var task = await _context.ToDoTasks.FindAsync(updatedTask.Id);
            if (task == null) return NotFound();

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return Challenge();

            if (task.CreatorId != currentUser.Id && !IsAdmin())
            {
                TempData["ErrorMessage"] = "Редактировать задачу может только её создатель.";
                return RedirectToAction(nameof(Details), new { id = task.Id });
            }
            
            task.Title = updatedTask.Title;
            task.Description = updatedTask.Description;
            task.Priority = updatedTask.Priority;

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Изменения сохранены";
            return RedirectToAction(nameof(Details), new { id = task.Id });
        }
        
        public async Task<IActionResult> Delete(int id)
        {
            var task = await _context.ToDoTasks
                .Include(t => t.Creator)
                .Include(t => t.Executor)
                .FirstOrDefaultAsync(t => t.Id == id);
            if (task == null) return NotFound();

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return Challenge();

            if (task.CreatorId != currentUser.Id && !IsAdmin())
            {
                TempData["ErrorMessage"] = "Удалить задачу может только её создатель.";
                return RedirectToAction(nameof(Details), new { id });
            }

            if (task.Status == TaskStatus.Открыта && !IsAdmin())
            {
                ModelState.AddModelError("", "Открытую задачу нельзя удалить");
                return View("Details", task);
            }

            _context.ToDoTasks.Remove(task);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Задача удалена";
            return RedirectToAction(nameof(Index));
        }

        
        public async Task<IActionResult> Take(int id)
        {
            var task = await _context.ToDoTasks.FindAsync(id);
            if (task == null) return NotFound();

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return Challenge();

            if (task.ExecutorId == null && task.Status == TaskStatus.Новая && task.CreatorId != currentUser.Id)
            {
                task.ExecutorId = currentUser.Id;
                task.Status = TaskStatus.Открыта;
                task.OpenedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Вы взяли задачу";
                return RedirectToAction(nameof(Details), new { id });
            }

            TempData["ErrorMessage"] = "Нельзя взять эту задачу.";
            return RedirectToAction(nameof(Details), new { id });
        }
        
        public async Task<IActionResult> AcceptAsExecutor(int id)
        {
            var task = await _context.ToDoTasks.FindAsync(id);
            if (task == null) return NotFound();

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return Challenge();

            if (task.CreatorId == currentUser.Id && task.ExecutorId == null)
            {
                task.ExecutorId = currentUser.Id;
                task.Status = TaskStatus.Открыта;
                task.OpenedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Вы приняли задачу как исполнитель";
                return RedirectToAction(nameof(Details), new { id });
            }

            TempData["ErrorMessage"] = "Нельзя принять задачу.";
            return RedirectToAction(nameof(Details), new { id });
        }
        
        public async Task<IActionResult> MyCreated()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return Challenge();

            var tasks = await _context.ToDoTasks
                .Include(t => t.Creator)
                .Include(t => t.Executor)
                .Where(t => t.CreatorId == currentUser.Id)
                .ToListAsync();

            return View("Index", tasks);
        }
        
        public async Task<IActionResult> MyAssigned()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return Challenge();

            var tasks = await _context.ToDoTasks
                .Include(t => t.Creator)
                .Include(t => t.Executor)
                .Where(t => t.ExecutorId == currentUser.Id)
                .ToListAsync();

            return View("Index", tasks);
        }

        public async Task<IActionResult> FreeTasks()
        {
            var tasks = await _context.ToDoTasks
                .Include(t => t.Creator)
                .Include(t => t.Executor)
                .Where(t => t.ExecutorId == null)
                .ToListAsync();

            return View("Index", tasks);
        }
    }
}
