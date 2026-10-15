
using System.ComponentModel.DataAnnotations;

namespace RaceDay.Api.DTOs
{
    public class SaveResultRequest
    {
        [Range(1, int.MaxValue)]
        public int EnrollmentId { get; set; }

        [Required]
        public TimeSpan? FinishTime { get; set; }

        [Range(1, int.MaxValue)]
        public int? Position { get; set; }

        [Range(0.01, 9999.99)]
        public decimal? Pace { get; set; }

        public string? Status { get; set; }
    }
}
