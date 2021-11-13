using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FeedbackSystemAPI.Models;
using System.Security.Claims;

namespace FeedbackSystemAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssignTasksController : ControllerBase
    {
        private readonly FeedbacSystemkDBContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public AssignTasksController(FeedbacSystemkDBContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        protected String GetCurrentUserId()
        {
            //var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            return userId;
        }

        // GET: api/AssignTasks
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AssignTask>>> GetAssignTasks()
        {
            return await _context.AssignTasks.ToListAsync();
        }

        // GET: api/AssignTasks/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AssignTask>> GetAssignTask(string id)
        {
            var assignTask = await _context.AssignTasks.FindAsync(id);

            if (assignTask == null)
            {
                return NotFound();
            }

            return assignTask;
        }

        [HttpGet("GetTaskUsers")]
        public async Task<ActionResult<User>> GetTaskUsers()
        {
            string UserId = GetCurrentUserId().ToString();
            return await _context.Users.Include(d => d.AssignTasks).ThenInclude(AssignTask => AssignTask.Task)
                .ThenInclude(Task => Task.Feedback)
                .ThenInclude(Feedback => Feedback.Device)
                .Where(d => d.UserId == UserId).FirstAsync();
        }

        [HttpGet("GetInfoTaskInAssingtask")]
        public async Task<ActionResult<IEnumerable<AssignTask>>> GetAss()
        {
            string UserId = GetCurrentUserId().ToString();
            return await _context.AssignTasks.Include(d => d.Task).ThenInclude(Task => Task.Feedback)
                .Where(d => d.EmployeeId == UserId).ToListAsync();
        }


        // PUT: api/AssignTasks/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAssignTask(string id, AssignTask assignTask)
        {
            if (id != assignTask.AssignId)
            {
                return BadRequest();
            }

            _context.Entry(assignTask).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AssignTaskExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/AssignTasks
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<AssignTask>> PostAssignTask(AssignTask assignTask)
        {
            _context.AssignTasks.Add(assignTask);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (AssignTaskExists(assignTask.AssignId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetAssignTask", new { id = assignTask.AssignId }, assignTask);
        }

        // DELETE: api/AssignTasks/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAssignTask(string id)
        {
            var assignTask = await _context.AssignTasks.FindAsync(id);
            if (assignTask == null)
            {
                return NotFound();
            }

            _context.AssignTasks.Remove(assignTask);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AssignTaskExists(string id)
        {
            return _context.AssignTasks.Any(e => e.AssignId == id);
        }
    }
}
