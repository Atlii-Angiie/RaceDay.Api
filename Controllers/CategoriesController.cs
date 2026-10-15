
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
    public class CategoriesController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public CategoriesController(RaceDayDbContext context)
        {
            _context = context;
        }

        // GET: api/Categories
        // Both roles can view categories.
        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _context.Categories
                .AsNoTracking()
                .ToListAsync();

            return Ok(categories);
        }

        // GET: api/Categories/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategory(int id)
        {
            var category = await _context.Categories
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
            {
                return NotFound("Category not found.");
            }

            return Ok(category);
        }

        // POST: api/Categories
        // Only Organisers can create categories.
        [HttpPost]
        [Authorize(Roles = Roles.Organiser)]
        public async Task<IActionResult> CreateCategory(
            CreateCategoryRequest request)
        {
            string name = request.Name.Trim();

            if (string.IsNullOrWhiteSpace(name))
            {
                return BadRequest("Category name is required.");
            }

            bool exists = await _context.Categories
                .AnyAsync(c => c.Name == name);

            if (exists)
            {
                return Conflict("A category with this name already exists.");
            }

            var category = new Category
            {
                Name = name,
                Description = request.Description
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetCategory),
                new { id = category.Id },
                category);
        }

        // PUT: api/Categories/5
        // Only Organisers can update categories.
        [HttpPut("{id}")]
        [Authorize(Roles = Roles.Organiser)]
        public async Task<IActionResult> UpdateCategory(
            int id,
            CreateCategoryRequest request)
        {
            var category = await _context.Categories.FindAsync(id);

            if (category == null)
            {
                return NotFound("Category not found.");
            }

            string name = request.Name.Trim();

            if (string.IsNullOrWhiteSpace(name))
            {
                return BadRequest("Category name is required.");
            }

            bool duplicateName = await _context.Categories
                .AnyAsync(c => c.Name == name && c.Id != id);

            if (duplicateName)
            {
                return Conflict("A category with this name already exists.");
            }

            category.Name = name;
            category.Description = request.Description;

            await _context.SaveChangesAsync();

            return Ok(category);
        }

        // DELETE: api/Categories/5
        // Prevent deletion if the category is in use.
        [HttpDelete("{id}")]
        [Authorize(Roles = Roles.Organiser)]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);

            if (category == null)
            {
                return NotFound("Category not found.");
            }

            bool linkedToEvent = await _context.EventCategories
                .AnyAsync(ec => ec.CategoryId == id);

            bool usedByEnrollment = await _context.Enrollments
                .AnyAsync(e => e.CategoryId == id);

            if (linkedToEvent || usedByEnrollment)
            {
                return Conflict(
                    "Cannot delete a category that is in use.");
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/Categories/5/events/1
        // Link an existing category to an event.
        [HttpPost("{categoryId}/events/{eventId}")]
        [Authorize(Roles = Roles.Organiser)]
        public async Task<IActionResult> LinkCategoryToEvent(
            int categoryId,
            int eventId)
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

            bool categoryExists = await _context.Categories
                .AnyAsync(c => c.Id == categoryId);

            if (!categoryExists)
            {
                return NotFound("Category not found.");
            }

            bool alreadyLinked = await _context.EventCategories
                .AnyAsync(ec =>
                    ec.EventId == eventId &&
                    ec.CategoryId == categoryId);

            if (alreadyLinked)
            {
                return Conflict(
                    "Category is already linked to this event.");
            }

            var eventCategory = new EventCategory
            {
                EventId = eventId,
                CategoryId = categoryId
            };

            _context.EventCategories.Add(eventCategory);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                Message = "Category linked to event successfully.",
                EventId = eventId,
                CategoryId = categoryId
            });
        }

        // GET: api/Categories/events/1
        // View categories available for a specific event.
        [HttpGet("events/{eventId}")]
        public async Task<IActionResult> GetEventCategories(int eventId)
        {
            bool eventExists = await _context.Events
                .AnyAsync(e => e.Id == eventId);

            if (!eventExists)
            {
                return NotFound("Event not found.");
            }

            var categories = await _context.EventCategories
                .Where(ec => ec.EventId == eventId)
                .Join(
                    _context.Categories,
                    ec => ec.CategoryId,
                    c => c.Id,
                    (ec, c) => new
                    {
                        c.Id,
                        c.Name,
                        c.Description
                    })
                .ToListAsync();

            return Ok(categories);
        }

        // DELETE: api/Categories/5/events/1
        // Remove a category from an event.
        [HttpDelete("{categoryId}/events/{eventId}")]
        [Authorize(Roles = Roles.Organiser)]
        public async Task<IActionResult> UnlinkCategoryFromEvent(
            int categoryId,
            int eventId)
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

            var eventCategory = await _context.EventCategories
                .FirstOrDefaultAsync(ec =>
                    ec.EventId == eventId &&
                    ec.CategoryId == categoryId);

            if (eventCategory == null)
            {
                return NotFound("Category is not linked to this event.");
            }

            bool hasEnrollments = await _context.Enrollments
                .AnyAsync(e =>
                    e.EventId == eventId &&
                    e.CategoryId == categoryId);

            if (hasEnrollments)
            {
                return Conflict(
                    "Cannot unlink a category with enrollments.");
            }

            _context.EventCategories.Remove(eventCategory);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
