
namespace RaceDay.Api.Models
{
    public class Event
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public DateOnly EventDate { get; set; }

        public TimeOnly? StartTime { get; set; }

        public string Location { get; set; } = string.Empty;

        public decimal? Distance { get; set; }
        public decimal? EntryFee { get; set; }

        public int EventTypeId { get; set; }

      
        public int OrganizerId { get; set; }

        public DateOnly? RegistrationDeadline { get; set; }

        public int? MaxParticipants { get; set; }

        public string Status { get; set; } = string.Empty;
    }
}
