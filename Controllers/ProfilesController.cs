
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using System.Security.Claims;

namespace RaceDay.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ProfilesController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public ProfilesController(RaceDayDbContext context)
        {
            _context = context;
        }

        // GET: api/Profiles/me
        // View the logged-in user's profile.
        [HttpGet("me")]
        public async Task<IActionResult> GetMyProfile()
        {
            string? userId = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (!int.TryParse(userId, out int currentUserId))
            {
                return Unauthorized();
            }

            var profile = await _context.Users
                .AsNoTracking()
                .Where(u => u.Id == currentUserId)
                .Select(u => new
                {
                    u.Id,
                    u.Name,
                    u.Surname,
                    u.Email,
                    u.Phone,
                    u.Location,
                    u.Role
                })
                .FirstOrDefaultAsync();

            if (profile == null)
            {
                return NotFound("User profile not found.");
            }

            return Ok(profile);
        }
    }
}
