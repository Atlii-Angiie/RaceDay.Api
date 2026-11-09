
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using System.Security.Claims;
using Azure.Storage.Blobs.Models;

namespace RaceDay.Api.Controllers
{
    [ApiController]
    [Route("api/events")]
    [Authorize(Roles = "Organiser")]
    public class EventImagesController : ControllerBase
    {
        private readonly RaceDayDbContext _context;
        private readonly BlobServiceClient _blobServiceClient;

        public EventImagesController(
            RaceDayDbContext context,
            BlobServiceClient blobServiceClient)
        {
            _context = context;
            _blobServiceClient = blobServiceClient;
        }


        [HttpPost("{id:int}/image")]
        [Consumes("multipart/form-data")]
        [RequestSizeLimit(5 * 1024 * 1024)]

        public async Task<IActionResult> UploadEventImage(
           int id,
             IFormFile image)

        {
            // Find the event.
            var raceEvent = await _context.Events.FindAsync(id);

            if (raceEvent == null)
            {
                return NotFound("Event not found.");
            }

            // Identify the logged-in Organiser.
            string? userId = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (!int.TryParse(userId, out int organiserId))
            {
                return Unauthorized();
            }

            // Only the event owner can upload its image.
            if (raceEvent.OrganizerId != organiserId)
            {
                return Forbid();
            }

            // Validate the image.
            if (image == null || image.Length == 0)
            {
                return BadRequest("Please select an image.");
            }

            if (image.Length > 5 * 1024 * 1024)
            {
                return BadRequest("Image must be 5 MB or smaller.");
            }

            // Validate the actual file signature.
            var signatures = new Dictionary<string, byte[]>
            {
                ["image/jpeg"] = new byte[] { 0xFF, 0xD8, 0xFF },
                ["image/png"] = new byte[]
                {
            0x89, 0x50, 0x4E, 0x47,
            0x0D, 0x0A, 0x1A, 0x0A
                }
            };

            await using var stream = image.OpenReadStream();

            var header = new byte[8];
            int bytesRead = await stream.ReadAsync(header);

            string? contentType = signatures
                .FirstOrDefault(s =>
                    bytesRead >= s.Value.Length &&
                    header.Take(s.Value.Length)
                        .SequenceEqual(s.Value))
                .Key;

            if (contentType == null)
            {
                return BadRequest("Only JPEG and PNG images are allowed.");
            }

            stream.Position = 0;

            // Generate a unique blob name.
            string extension = contentType == "image/png"
                ? ".png"
                : ".jpg";

            string blobName =
                $"events/{id}/{Guid.NewGuid():N}{extension}";

            // Upload to the private Azure container.
            var container = _blobServiceClient
                .GetBlobContainerClient("race-images");

            var blob = container.GetBlobClient(blobName);

            await blob.UploadAsync(
                stream,
                new BlobUploadOptions
                {
                    HttpHeaders = new BlobHttpHeaders
                    {
                        ContentType = contentType
                    }
                });

            // Save the Azure Blob URL to SQL Server.
            raceEvent.ImageUrl = blob.Uri.ToString();

            await _context.SaveChangesAsync();

            return Ok(new
            {
                Message = "Event image uploaded successfully.",
                ImageUrl = raceEvent.ImageUrl
            });
        }
    [HttpGet("{id:int}/image")]
        [AllowAnonymous]
        public async Task<IActionResult> GetEventImage(int id)
        {
            var raceEvent = await _context.Events.FindAsync(id);

            if (raceEvent == null || string.IsNullOrWhiteSpace(raceEvent.ImageUrl))
                return NotFound();

            var container = _blobServiceClient.GetBlobContainerClient("race-images");

            var blobUri = new Uri(raceEvent.ImageUrl);
            var blobName = Uri.UnescapeDataString(
                blobUri.AbsolutePath.Split("/race-images/", 2)[1]);

            var blob = container.GetBlobClient(blobName);

            if (!await blob.ExistsAsync())
                return NotFound();

            var download = await blob.DownloadStreamingAsync();

            return File(
                download.Value.Content,
                download.Value.Details.ContentType ?? "application/octet-stream");
        }
    }
    }
