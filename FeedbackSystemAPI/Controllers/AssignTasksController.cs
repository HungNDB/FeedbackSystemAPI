using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FeedbackSystemAPI.Models;
using System.Security.Claims;
using FeedbackSystemAPI.Services;

namespace FeedbackSystemAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssignTasksController : ControllerBase
    {
        private readonly IMailService mailService;
        private readonly FeedbacSystemkDBContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public AssignTasksController(FeedbacSystemkDBContext context, IHttpContextAccessor httpContextAccessor, IMailService mailService)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            this.mailService = mailService;
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
            return await _context.AssignTasks.Where(d => d.IsDelete != "true")
                .ToListAsync();
        }

        // GET: api/AssignTasks/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AssignTask>> GetAssignTask(string id)
        {
            var assignTask = await _context.AssignTasks.Where(d => d.EmployeeId == id && d.IsDelete != "true")
                .MinAsync();

            if (assignTask == null)
            {
                return NotFound();
            }

            return assignTask;
        }

    
        [HttpGet("GetInfoTaskInAssingtask")]
        public async Task<ActionResult<IEnumerable<AssignTask>>> GetAss()
        {
            string UserId = GetCurrentUserId().ToString();
            return await _context.AssignTasks.Include(d => d.Task).ThenInclude(Task => Task.Feedback).ThenInclude(Feedback => Feedback.Device)
                .Where(d => d.EmployeeId == UserId && d.IsDelete != "true").ToListAsync();
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
        [HttpPost("{idTask}/{ididEmp}/{idFb}/CreatAssignTask")]
        public async Task<ActionResult<AssignTask>> CreatAssignTask(string idTask, string idEmp)
        {
            AssignTask assignTask = new AssignTask();
            assignTask.TaskId = idTask;
            assignTask.EmployeeId = idEmp;

            var userbyid = await _context.Users.FindAsync(idEmp);
            string toMail = userbyid.Email;

            var fb = await _context.Tasks.Include(d => d.Feedback).ThenInclude(d => d.Device).ThenInclude(d => d.Location)
                .Where(d => d.TaskId == idTask).FirstAsync();

            string bodyMess = "Thiết bị: " + fb.Feedback.Device.Name + " Tại : " +
                fb.Feedback.Device.Location.LocatitonName + " có vấn đề, hư hỏng, không sử dụng được và cần bạn đến xem sét và sửa chữa";

            _context.AssignTasks.Add(assignTask);
            try
            {
                await _context.SaveChangesAsync();
                await mailService.SendEmailAsync(toMail, bodyMess);
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
            var t = await _context.AssignTasks.FindAsync(id);

            if (id != t.AssignId)
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

        private bool AssignTaskExists(string id)
        {
            return _context.AssignTasks.Any(e => e.AssignId == id);
        }
    }
}
