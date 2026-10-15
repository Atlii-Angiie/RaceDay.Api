
namespace RaceDay.Api.Models
{
    public class Enrollment
    {
        public int Id { get; set; }

        public int EventId { get; set; }

        public int UserId { get; set; }

        public int CategoryId { get; set; }
        public DateTime RegistrationDate { get; set; }

        public string? RaceNumber { get; set; }

        public string Status { get; set; } = string.Empty;
    }
}
