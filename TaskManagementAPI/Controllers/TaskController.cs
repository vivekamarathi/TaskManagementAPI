
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagementAPI.Data;
using TaskManagementAPI.Models;
using TaskManagementAPI.Services;

namespace TaskManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly RedisCacheService _cacheService;
        public TaskController(AppDbContext context, RedisCacheService cacheService)
        {
            _context = context;
            _cacheService = cacheService;
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<TaskTable>>> GetTasksByUser(int userId)
        {
            string cacheKey = $"tasks_user_{userId}";
            var cachedTasks = await _cacheService.GetAsync<List<TaskTable>>(cacheKey);

            if (cachedTasks != null) return cachedTasks;

            var tasks = await _context.Tasks.Where(t => t.AssignedUserId == userId).ToListAsync();
            await _cacheService.SetAsync(cacheKey, tasks, TimeSpan.FromMinutes(10));

            return tasks;
        }

        // GET: api/tasks
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskTable>>> GetTasks()
        {
            return await _context.Tasks.ToListAsync();
        }

        // GET: api/tasks/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<TaskTable>> GetTask(int id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task == null) return NotFound();
            return task;
        }

        // POST: api/tasks
        [HttpPost]
        public async Task<ActionResult<Task>> CreateTask(TaskTable task)
        {
            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetTask), new { id = task.Id }, task);
        }

        // PUT: api/tasks/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(int id, TaskTable task)
        {
            if (id != task.Id) return BadRequest();
            _context.Entry(task).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/tasks/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task == null) return NotFound();
            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
