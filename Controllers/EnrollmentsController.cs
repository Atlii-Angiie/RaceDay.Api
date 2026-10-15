
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Constants;
using RaceDay.Api.Data;
using RaceDay.Api.DTOs;
using RaceDay.Api.Models;
using System.Security.Claims;

namespace RaceDay.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class EnrollmentsController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public EnrollmentsController(RaceDayDbContext context)
        {
            _context = context;
        }

        // POST: api/Enrollments
        // Participants enroll in an event and select a category.
        [HttpPost]
        [Authorize(Roles = Roles.Participant)]
        public async Task<IActionResult> CreateEnrollment(
            CreateEnrollmentRequest request)
        {
            string? userId = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (!int.TryParse(userId, out int participantId))
            {
                return Unauthorized();
            }

            var raceEvent = await _context.Events
                .FindAsync(request.EventId);

            if (raceEvent == null)
            {
                return NotFound("Event not found.");
            }

            if (raceEvent.Status != "Open")
            {
                return Conflict("Event is not open for enrollment.");
            }

            if (raceEvent.RegistrationDeadline.HasValue &&
                raceEvent.RegistrationDeadline.Value <
                    DateOnly.FromDateTime(DateTime.UtcNow))
            {
                return Conflict("Registration deadline has passed.");
            }

            bool categoryAvailable = await _context.EventCategories
                .AnyAsync(ec =>
                    ec.EventId == request.EventId &&
                    ec.CategoryId == request.CategoryId);

            if (!categoryAvailable)
            {
                return BadRequest(
                    "The selected category is not available for this event.");
            }

            bool alreadyEnrolled = await _context.Enrollments
                .AnyAsync(e =>
                    e.EventId == request.EventId &&
                    e.UserId == participantId &&
                    e.Status != "Cancelled");

            if (alreadyEnrolled)
            {
                return Conflict(
                    "You are already enrolled in this event.");
            }

            if (raceEvent.MaxParticipants.HasValue)
            {
                int enrollmentCount = await _context.Enrollments
                    .CountAsync(e =>
                        e.EventId == request.EventId &&
                        e.Status != "Cancelled");

                if (enrollmentCount >= raceEvent.MaxParticipants.Value)
                {
                    return Conflict("This event is full.");
                }
            }

            var enrollment = new Enrollment
            {
                EventId = request.EventId,
                CategoryId = request.CategoryId,
                UserId = participantId,
                RegistrationDate = DateTime.UtcNow,
                Status = "Registered"
            };

            _context.Enrollments.Add(enrollment);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetEnrollment),
                new { id = enrollment.Id },
                new
                {
                    enrollment.Id,
                    enrollment.EventId,
                    enrollment.CategoryId,
                    enrollment.UserId,
                    enrollment.RegistrationDate,
                    enrollment.Status
                });
        }

        // GET: api/Enrollments/5
        // Participants see their own enrollment.
        // Organisers see enrollments for events they own.
        [HttpGet("{id}")]
        public async Task<IActionResult> GetEnrollment(int id)
        {
            string? userId = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (!int.TryParse(userId, out int currentUserId))
            {
                return Unauthorized();
            }

            var enrollment = await _context.Enrollments
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == id);

            if (enrollment == null)
            {
                return NotFound("Enrollment not found.");
            }

            bool isParticipantOwner =
                User.IsInRole(Roles.Participant) &&
                enrollment.UserId == currentUserId;

            bool isEventOrganiser =
                User.IsInRole(Roles.Organiser) &&
                await _context.Events.AnyAsync(e =>
                    e.Id == enrollment.EventId &&
                    e.OrganizerId == currentUserId);

            if (!isParticipantOwner && !isEventOrganiser)
            {
                return Forbid();
            }

            return Ok(enrollment);
        }

        // GET: api/Enrollments/me
        // Participants view their own enrollments.
        [HttpGet("me")]
        [Authorize(Roles = Roles.Participant)]
        public async Task<IActionResult> GetMyEnrollments()
        {
            string? userId = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (!int.TryParse(userId, out int participantId))
            {
                return Unauthorized();
            }

            var enrollments = await _context.Enrollments
                .AsNoTracking()
                .Where(e => e.UserId == participantId)
                .OrderByDescending(e => e.RegistrationDate)
                .ToListAsync();

            return Ok(enrollments);
        }

        // GET: api/Enrollments/event/5
        // Event owners view participants enrolled in their event.
        [HttpGet("event/{eventId}")]
        [Authorize(Roles = Roles.Organiser)]
        public async Task<IActionResult> GetEventEnrollments(int eventId)
        {
            string? userId = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (!int.TryParse(userId, out int organiserId))
            {
                return Unauthorized();
            }

            var raceEvent = await _context.Events.FindAsync(eventId);

            if (raceEvent == null)
            {
                return NotFound("Event not found.");
            }

            if (raceEvent.OrganizerId != organiserId)
            {
                return Forbid();
            }

            var enrollments = await _context.Enrollments
                .AsNoTracking()
                .Where(e => e.EventId == eventId)
                .OrderBy(e => e.RegistrationDate)
                .ToListAsync();

            return Ok(enrollments);
        }

        // DELETE: api/Enrollments/5
        // Participants cancel their own enrollment.
        [HttpDelete("{id}")]
        [Authorize(Roles = Roles.Participant)]
        public async Task<IActionResult> CancelEnrollment(int id)
        {
            string? userId = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (!int.TryParse(userId, out int participantId))
            {
                return Unauthorized();
            }

            var enrollment = await _context.Enrollments.FindAsync(id);

            if (enrollment == null)
            {
                return NotFound("Enrollment not found.");
            }

            if (enrollment.UserId != participantId)
            {
                return Forbid();
            }

            if (enrollment.Status == "Cancelled")
            {
                return Conflict("Enrollment is already cancelled.");
            }

            bool hasResult = await _context.Results
                .AnyAsync(r => r.EnrollmentId == id);

            if (hasResult)
            {
                return Conflict(
                    "Cannot cancel an enrollment with a recorded result.");
            }

            enrollment.Status = "Cancelled";
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
