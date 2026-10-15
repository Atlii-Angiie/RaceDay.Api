
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using RaceDay.Api.DTOs;

namespace RaceDay.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class EventsController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public EventsController(RaceDayDbContext context)
        {
            _context = context;
        }

        // GET: api/events
        [HttpGet]
        public async Task<IActionResult> GetEvents()
        {
            var events = await _context.Events.ToListAsync();
            return Ok(events);
        }

        // GET: api/events/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetEventById(int id)
        {
            var raceEvent = await _context.Events.FindAsync(id);

            if (raceEvent == null)
            {
                return NotFound();
            }

            return Ok(raceEvent);
        }

        // POST: api/events
        [HttpPost]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> CreateEvent(CreateEventRequest request)
        {
            // Identify the Organiser from their JWT.
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userId, out int organiserId))
            {
                return Unauthorized();
            }

            // NEW VALIDATION: Event name is required.
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return BadRequest("Event name is required.");
            }

            // NEW VALIDATION: Event location is required.
            if (string.IsNullOrWhiteSpace(request.Location))
            {
                return BadRequest("Event location is required.");
            }

            // Registration must close on or before the event date.
            if (request.RegistrationDeadline > request.EventDate)
            {
                return BadRequest(
                    "Registration deadline cannot be after the event date.");
            }

            // Maximum participants must be greater than zero.
            if (request.MaxParticipants <= 0)
            {
                return BadRequest(
                    "Maximum participants must be greater than zero.");
            }

            // Distance must be greater than zero.
            if (request.Distance <= 0)
            {
                return BadRequest(
                    "Event distance must be greater than zero.");
            }

            // Entry fee may be zero, but cannot be negative.
            if (request.EntryFee < 0)
            {
                return BadRequest(
                    "Entry fee cannot be negative.");
            }

            // Check that the event type exists.
            bool eventTypeExists = await _context.EventTypes
                .AnyAsync(t => t.Id == request.EventTypeId);

            if (!eventTypeExists)
            {
                return BadRequest("Invalid event type.");
            }

            // Create the event.
            var raceEvent = new RaceDay.Api.Models.Event
            {
                Name = request.Name.Trim(),
                Description = request.Description,
                EventDate = request.EventDate,
                StartTime = request.StartTime,
                Location = request.Location.Trim(),
                Distance = request.Distance,
                EntryFee = request.EntryFee,
                EventTypeId = request.EventTypeId,
                OrganizerId = organiserId,
                RegistrationDeadline = request.RegistrationDeadline,
                MaxParticipants = request.MaxParticipants,
                Status = "Open"
            };

            _context.Events.Add(raceEvent);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetEventById),
                new { id = raceEvent.Id },
                new
                {
                    raceEvent.Id,
                    raceEvent.Name,
                    raceEvent.EventDate,
                    raceEvent.Location,
                    raceEvent.OrganizerId,
                    raceEvent.Status
                }
            );
        }

        // PUT: api/events/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> UpdateEvent(
            int id,
            UpdateEventRequest request)
        {
            // Find the event being updated.
            var raceEvent = await _context.Events.FindAsync(id);

            if (raceEvent == null)
            {
                return NotFound("Event not found.");
            }

            // Identify the logged-in Organiser.
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userId, out int organiserId))
            {
                return Unauthorized();
            }

            // Only the Organiser who created the event may edit it.
            if (raceEvent.OrganizerId != organiserId)
            {
                return Forbid();
            }

            // NEW VALIDATION: Event name is required.
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return BadRequest("Event name is required.");
            }

            // NEW VALIDATION: Event location is required.
            if (string.IsNullOrWhiteSpace(request.Location))
            {
                return BadRequest("Event location is required.");
            }

            // Registration must close on or before the event date.
            if (request.RegistrationDeadline > request.EventDate)
            {
                return BadRequest(
                    "Registration deadline cannot be after the event date.");
            }

            // Maximum participants must be greater than zero.
            if (request.MaxParticipants <= 0)
            {
                return BadRequest(
                    "Maximum participants must be greater than zero.");
            }

            // Distance must be greater than zero.
            if (request.Distance <= 0)
            {
                return BadRequest(
                    "Event distance must be greater than zero.");
            }

            // Entry fee may be zero, but cannot be negative.
            if (request.EntryFee < 0)
            {
                return BadRequest(
                    "Entry fee cannot be negative.");
            }

            // Check that the requested event type exists.
            bool eventTypeExists = await _context.EventTypes
                .AnyAsync(t => t.Id == request.EventTypeId);

            if (!eventTypeExists)
            {
                return BadRequest("Invalid event type.");
            }

            // Update the event details.
            raceEvent.Name = request.Name.Trim();
            raceEvent.Description = request.Description;
            raceEvent.EventDate = request.EventDate;
            raceEvent.StartTime = request.StartTime;
            raceEvent.Location = request.Location.Trim();
            raceEvent.Distance = request.Distance;
            raceEvent.EntryFee = request.EntryFee;
            raceEvent.EventTypeId = request.EventTypeId;
            raceEvent.RegistrationDeadline = request.RegistrationDeadline;
            raceEvent.MaxParticipants = request.MaxParticipants;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                raceEvent.Id,
                raceEvent.Name,
                raceEvent.EventDate,
                raceEvent.Location,
                raceEvent.Distance,
                raceEvent.OrganizerId,
                raceEvent.Status
            });
        }

        // DELETE: api/events/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            // Find the event.
            var raceEvent = await _context.Events.FindAsync(id);

            if (raceEvent == null)
            {
                return NotFound("Event not found.");
            }

            // Identify the logged-in Organiser.
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userId, out int organiserId))
            {
                return Unauthorized();
            }

            // Only the event owner can delete it.
            if (raceEvent.OrganizerId != organiserId)
            {
                return Forbid();
            }

            // Prevent deletion when participants have enrolled.
            bool hasEnrollments = await _context.Enrollments
                .AnyAsync(e => e.EventId == id);

            if (hasEnrollments)
            {
                return Conflict("Cannot delete an event with enrollments.");
            }

            _context.Events.Remove(raceEvent);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
