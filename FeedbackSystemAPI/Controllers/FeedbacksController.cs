using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FeedbackSystemAPI.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace FeedbackSystemAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeedbacksController : ControllerBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly FeedbacSystemkDBContext _context;

        public FeedbacksController(FeedbacSystemkDBContext context, IHttpContextAccessor httpContextAccessor)
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

        // GET: api/Feedbacks
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Feedback>>> GetFeedbacks()
        {
            return await _context.Feedbacks.Include(d => d.Device).ThenInclude(Device => Device.Location)
                        .ToListAsync();
        }

        [HttpGet("GetFeedbackAll")]
        public async Task<ActionResult<IEnumerable<Feedback>>> GetFeedbackAll()
        {
            return await _context.Feedbacks.ToListAsync();
        }

        [HttpGet("GetFeedbacksCompleted")]
        public async Task<ActionResult<IEnumerable<Feedback>>> GetFeedbacksCompleted()
        {
            return await _context.Feedbacks.Include(d => d.Device).ThenInclude(Device => Device.Location)
                        .Where(f => f.Status == "Completed")
                        .ToListAsync();
        }

        [HttpGet("GetFeedbacksProcessing")]
        public async Task<ActionResult<IEnumerable<Feedback>>> GetFeedbacksProcessing()
        {
            return await _context.Feedbacks.Include(d => d.Device).ThenInclude(Device => Device.Location)
                        .Where(f => f.Status == "Processing")
                        .ToListAsync();
        }

        // GET: api/Feedbacks/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Feedback>> GetFeedback(string id)
        {
            var feedback = await _context.Feedbacks.FindAsync(id);

            if (feedback == null)
            {
                return NotFound();
            }

            return feedback;
        }

        [HttpGet("{id}/GetDevieceF")]
        public async Task<ActionResult<Feedback>> GetDevieceF([FromQuery] string id)
        {
            return await _context.Feedbacks.Include(d => d.Device).ThenInclude(Device => Device.Location)
                .Where(d => d.UserId == id).FirstAsync();
        }


        [HttpGet("{UserId}/Feedback")]
        public async Task<ActionResult<IEnumerable<Feedback>>> GetDevicesbys([FromQuery] string UserId)
        {
            return await _context.Feedbacks.Where(f => f.UserId == UserId).ToListAsync();
        }

        // PUT: api/Feedbacks/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}/UpdateCompleted")]
        public async Task<IActionResult> PutFeedback(string id, Feedback feedback)
        {

            if (id != feedback.FeedbackId)
            {
                return BadRequest();
            }

            feedback.Status = "Completed";

            _context.Entry(feedback).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FeedbackExists(id))
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

        [HttpPut("{id}/UpdateProcessing")]
        public async Task<IActionResult> PutFeedbackProcessing(string id, Feedback feedback)
        {

            if (id != feedback.FeedbackId)
            {
                return BadRequest();
            }

            feedback.Status = "Processing";

            _context.Entry(feedback).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FeedbackExists(id))
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

        // POST: api/Feedbacks
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Feedback>> PostFeedback(Feedback feedback)
        {
            feedback.UserId = GetCurrentUserId().ToString();
            feedback.DateTime = DateTime.Now.ToString();
            feedback.Status = "Pending";
            _context.Feedbacks.Add(feedback);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (FeedbackExists(feedback.FeedbackId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetFeedback", new { id = feedback.FeedbackId }, feedback);
        }


       
        // DELETE: api/Feedbacks/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFeedback(string id)
        {
            var feedback = await _context.Feedbacks.FindAsync(id);
            if (feedback == null)
            {
                return NotFound();
            }

            _context.Feedbacks.Remove(feedback);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool FeedbackExists(string id)
        {
            return _context.Feedbacks.Any(e => e.FeedbackId == id);
        }
    }
}
