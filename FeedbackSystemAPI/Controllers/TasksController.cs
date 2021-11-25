using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FeedbackSystemAPI.Models;
using Task = FeedbackSystemAPI.Models.Task;
using FeedbackSystemAPI.Services;

namespace FeedbackSystemAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly FeedbacSystemkDBContext _context;
        private readonly IMailService mailService;
        public TasksController(FeedbacSystemkDBContext context, IMailService mailService)
        {
            _context = context;
            this.mailService = mailService;
        }

        // GET: api/Tasks
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Models.Task>>> GetTasks()
        {
            return await _context.Tasks.ToListAsync();
        }

        // GET: api/Tasks/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Models.Task>> GetTask(string id)
        {
            var task = await _context.Tasks.FindAsync(id);

            if (task == null)
            {
                return NotFound();
            }

            return task;
        }

        [HttpGet("GetTaskCompleted")]
        public async Task<ActionResult<IEnumerable<Task>>> GetTaskCompleted()
        {
            return await _context.Tasks
                        .Where(f => f.Status == "Completed" && f.IsDelete != "true")
                        .ToListAsync();
        }

        [HttpGet("GetTaskProcessing")]
        public async Task<ActionResult<IEnumerable<Task>>> GetTaskProcessing()
        {
             return await _context.Tasks
                        .Where(f => f.Status == "Processing" && f.IsDelete != "true")
                        .ToListAsync();
        }

        [HttpGet("GetTaskPending")]
        public async Task<ActionResult<IEnumerable<Task>>> GetFeedbacksPending()
        {
            return await _context.Tasks
                        .Where(f => f.Status == "Pending" && f.IsDelete != "true")
                        .ToListAsync();
        }

        // PUT: api/Tasks/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTask(string id, Task task)
        {
            if (id != task.TaskId)
            {
                return BadRequest();
            }

            _context.Entry(task).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TaskExists(id))
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



        // POST: api/Tasks
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Models.Task>> PostTask(Task task)
        {
            task.DateTime = DateTime.Now.ToString();
            task.Status = "Pending";
            _context.Tasks.Add(task);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (TaskExists(task.TaskId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetTask", new { id = task.TaskId }, task);
        }

        [HttpPost("{idFeedback}/PostTaskByFbID")]
        public async Task<ActionResult<Models.Task>> PostTaskByFbID(string idFb)
        {
            Task task = new Task();
            task.FeedbackId = idFb;
            task.DateTime = DateTime.Now.ToString();
            task.Status = "Pending";
            _context.Tasks.Add(task);
            try
            {
                await _context.SaveChangesAsync();
                
            }
            catch (DbUpdateException)
            {
                if (TaskExists(task.TaskId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetTask", new { id = task.TaskId }, task);
        }

        // DELETE: api/Tasks/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(string id)
        {
            var t = await _context.Tasks.FindAsync(id);

            if (id != t.TaskId)
            {
                return BadRequest();
            }

            t.IsDelete = "true";

            _context.Entry(t).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TaskExists(id))
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

        private bool TaskExists(string id)
        {
            return _context.Tasks.Any(e => e.TaskId == id);
        }
    }
}
