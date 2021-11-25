using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FeedbackSystemAPI.Models;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;

namespace FeedbackSystemAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly FeedbacSystemkDBContext _context;
        private readonly JwtIssuerOptions _jwtOptions;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UsersController(FeedbacSystemkDBContext context, IOptions<JwtIssuerOptions> jwtOptions, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _jwtOptions = jwtOptions.Value;
            _httpContextAccessor = httpContextAccessor;
        }

        protected String GetCurrentUserId()
        {
            //var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            return userId;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }

        //[HttpGet("GetInfoUser")]
        //public async Task<ActionResult<UserInfo>> GetUser()
        //{
        //    string UserId = GetCurrentUserId().ToString();
        //    var userbyid = await _context.Users.FindAsync(UserId);
        //    UserInfo a = new UserInfo(userbyid);

        //    if (a == null)
        //    {
        //        return NotFound();
        //    }

        //    return a;
        //}

        [HttpGet("GetInfoUser")]
        public async Task<ActionResult<IEnumerable<User>>> GetUser()
        {
            string UserId = GetCurrentUserId().ToString();
                  
            return await _context.Users.Where(d => d.UserId == UserId).ToListAsync();
        }

        [HttpGet("GetEmp")]
        public async Task<ActionResult<IEnumerable<User>>> GetEmp()
        {
           
            return await _context.Users
                .Where(a => a.RoleId == "1")
                .ToListAsync();
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(string id, User user)
        {
            if (id != user.UserId)
            {
                return BadRequest();
            }

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
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

        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult<UserWithToken>> Login([FromBody] User user)
        {
            user = await _context.Users
                .Include(u => u.Role)
                .Where(u => u.Email == user.Email && u.Password == user.Password).FirstOrDefaultAsync();
            if (user == null)
            {
                return null;
            }
            UserWithToken userWithToken = new UserWithToken(user);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, await _jwtOptions.JtiGenerator())
            };

            var jwt = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: claims,
                notBefore: _jwtOptions.NotBefore,
                expires: _jwtOptions.Expiration,
                signingCredentials: _jwtOptions.SigningCredentials
                );
            userWithToken.AccessToken = new JwtSecurityTokenHandler().WriteToken(jwt);
            return userWithToken;
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserExists(string id)
        {
            return _context.Users.Any(e => e.UserId == id);
        }
    }
}
