
using System.ComponentModel.DataAnnotations;

namespace RaceDay.Api.DTOs
{
    public class CreateEventRequest
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required]
        public DateOnly EventDate { get; set; }

        public TimeOnly? StartTime { get; set; }

        [Required]
        public string Location { get; set; } = string.Empty;

        [Range(0.01, 9999.99)]
        public decimal Distance { get; set; }

        public decimal? EntryFee { get; set; }

        [Range(1, int.MaxValue)]
        public int EventTypeId { get; set; }

        public DateOnly? RegistrationDeadline { get; set; }

        public int? MaxParticipants { get; set; }
    }
}
