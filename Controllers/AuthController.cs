
using Microsoft.AspNetCore.Mvc;
using RaceDay.Api.Data;
using RaceDay.Api.Services;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Constants;
using RaceDay.Api.DTOs;
using RaceDay.Api.Models;

namespace RaceDay.Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly RaceDayDbContext _context;
        private readonly PasswordService _passwordService;
        private readonly TokenService _tokenService;

        public AuthController(
            RaceDayDbContext context,
            PasswordService passwordService,
            TokenService tokenService)
        {
            _context = context;
            _passwordService = passwordService;
            _tokenService = tokenService;
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            // Only the two assignment roles are allowed.
            if (request.Role != Roles.Organiser &&
                request.Role != Roles.Participant)
            {
                return BadRequest("Role must be Organiser or Participant.");
            }

            // Prevent duplicate email registrations.
            string email = request.Email.Trim();

            bool emailExists = await _context.Users
                .AnyAsync(u => u.Email == email);

            if (emailExists)
            {
                return Conflict("An account with this email already exists.");
            }

            // Hash the password before saving the user.
            string passwordHash =
                _passwordService.HashPassword(request.Password);

            var user = new User
            {
                Name = request.Name.Trim(),
                Surname = request.Surname.Trim(),
                Email = email,
                PasswordHash = passwordHash,
                Role = request.Role
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Never return the password or password hash.
            return StatusCode(201, new
            {
                user.Id,
                user.Name,
                user.Surname,
                user.Email,
                user.Role
            });
        }

            [HttpPost("login")]
            public async Task<IActionResult> Login(LoginRequest request)
            {
                // Find the user by email.
                string email = request.Email.Trim();

                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == email);

                if (user == null)
                {
                    return Unauthorized("Invalid email or password.");
                }

                // Check the submitted password against the stored hash.
                bool passwordIsValid = _passwordService.VerifyPassword(
                    request.Password,
                    user.PasswordHash
                );

                if (!passwordIsValid)
                {
                    return Unauthorized("Invalid email or password.");
                }


            string token = _tokenService.GenerateToken(user);

            return Ok(new
            {
                AccessToken = token,
                TokenType = "Bearer",
                ExpiresIn = 3600,
                user.Id,
                user.Name,
                user.Email,
                user.Role
            });

        }
    }
}
