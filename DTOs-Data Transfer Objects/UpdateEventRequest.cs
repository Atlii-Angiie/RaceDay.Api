
using System.ComponentModel.DataAnnotations;

namespace RaceDay.Api.DTOs
{
    public class UpdateEventRequest
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public DateOnly EventDate { get; set; }

        public TimeOnly? StartTime { get; set; }

        [Required]
        public string Location { get; set; } = string.Empty;

        [Range(0.01, 9999.99)]
        public decimal Distance { get; set; }

        [Range(0, 99999999.99)]
        public decimal? EntryFee { get; set; }

        [Range(1, int.MaxValue)]
        public int EventTypeId { get; set; }

        public DateOnly? RegistrationDeadline { get; set; }

        [Range(1, int.MaxValue)]
        public int? MaxParticipants { get; set; }
    }
}
