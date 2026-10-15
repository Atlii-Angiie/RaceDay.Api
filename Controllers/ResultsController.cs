
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
    public class ResultsController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public ResultsController(RaceDayDbContext context)
        {
            _context = context;
        }

        // POST: api/Results
        // Organisers record or update finishing results.
        [HttpPost]
        [Authorize(Roles = Roles.Organiser)]
        public async Task<IActionResult> SaveResult(
            SaveResultRequest request)
        {
            string? userId = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (!int.TryParse(userId, out int organiserId))
            {
                return Unauthorized();
            }

            var enrollment = await _context.Enrollments
                .FindAsync(request.EnrollmentId);

            if (enrollment == null)
            {
                return NotFound("Enrollment not found.");
            }

            var raceEvent = await _context.Events
                .FindAsync(enrollment.EventId);

            if (raceEvent == null)
            {
                return NotFound("Event not found.");
            }

            if (raceEvent.OrganizerId != organiserId)
            {
                return Forbid();
            }

            if (enrollment.Status == "Cancelled")
            {
                return Conflict(
                    "Cannot record a result for a cancelled enrollment.");
            }

            if (request.FinishTime.HasValue &&
                (request.FinishTime.Value <= TimeSpan.Zero ||
                 request.FinishTime.Value >= TimeSpan.FromDays(1)))
            {
                return BadRequest(
                    "Finish time must be greater than zero and less than 24 hours.");
            }

            var result = await _context.Results
                .FirstOrDefaultAsync(r =>
                    r.EnrollmentId == request.EnrollmentId);

            bool isNewResult = result == null;

            if (result == null)
            {
                result = new Result
                {
                    EnrollmentId = request.EnrollmentId
                };

                _context.Results.Add(result);
            }

            result.FinishTime = request.FinishTime;
            result.Position = request.Position;
            result.Pace = request.Pace;
            result.Status = string.IsNullOrWhiteSpace(request.Status)
                ? "Finished"
                : request.Status.Trim();

            await _context.SaveChangesAsync();

            var response = new
            {
                result.Id,
                result.EnrollmentId,
                result.FinishTime,
                result.Position,
                result.Pace,
                result.Status
            };

            if (isNewResult)
            {
                return CreatedAtAction(
                    nameof(GetResult),
                    new { id = result.Id },
                    response);
            }

            return Ok(response);
        }

        // GET: api/Results/5
        // Participant can view their own result.
        // Organiser can view results for their events.
        [HttpGet("{id}")]
        public async Task<IActionResult> GetResult(int id)
        {
            string? userId = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (!int.TryParse(userId, out int currentUserId))
            {
                return Unauthorized();
            }

            var result = await _context.Results
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == id);

            if (result == null)
            {
                return NotFound("Result not found.");
            }

            var enrollment = await _context.Enrollments
                .AsNoTracking()
                .FirstOrDefaultAsync(e =>
                    e.Id == result.EnrollmentId);

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

            return Ok(result);
        }

        // GET: api/Results/me
        // Participants view all their results.
        [HttpGet("me")]
        [Authorize(Roles = Roles.Participant)]
        public async Task<IActionResult> GetMyResults()
        {
            string? userId = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (!int.TryParse(userId, out int participantId))
            {
                return Unauthorized();
            }

            var results = await _context.Results
                .AsNoTracking()
                .Join(
                    _context.Enrollments,
                    r => r.EnrollmentId,
                    e => e.Id,
                    (r, e) => new { Result = r, Enrollment = e })
                .Where(x => x.Enrollment.UserId == participantId)
                .Select(x => new
                {
                    x.Result.Id,
                    x.Result.EnrollmentId,
                    x.Enrollment.EventId,
                    x.Enrollment.CategoryId,
                    x.Result.FinishTime,
                    x.Result.Position,
                    x.Result.Pace,
                    x.Result.Status
                })
                .ToListAsync();

            return Ok(results);
        }

        // GET: api/Results/event/5
        // Organisers view results for their own events.
        [HttpGet("event/{eventId}")]
        [Authorize(Roles = Roles.Organiser)]
        public async Task<IActionResult> GetEventResults(int eventId)
        {
            string? userId = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (!int.TryParse(userId, out int organiserId))
            {
                return Unauthorized();
            }

            var raceEvent = await _context.Events
                .FindAsync(eventId);

            if (raceEvent == null)
            {
                return NotFound("Event not found.");
            }

            if (raceEvent.OrganizerId != organiserId)
            {
                return Forbid();
            }

            var results = await _context.Results
                .AsNoTracking()
                .Join(
                    _context.Enrollments,
                    r => r.EnrollmentId,
                    e => e.Id,
                    (r, e) => new { Result = r, Enrollment = e })
                .Where(x => x.Enrollment.EventId == eventId)
                .Select(x => new
                {
                    x.Result.Id,
                    x.Result.EnrollmentId,
                    x.Enrollment.UserId,
                    x.Enrollment.CategoryId,
                    x.Result.FinishTime,
                    x.Result.Position,
                    x.Result.Pace,
                    x.Result.Status
                })
                .OrderBy(x => x.Position)
                .ToListAsync();

            return Ok(results);
        }
    }
}

