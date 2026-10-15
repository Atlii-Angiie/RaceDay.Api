
namespace RaceDay.Api.Models
{
    public class EventType
    {
        public int Id { get; set; }

        public string EventName { get; set; } = string.Empty;

        public string? Description { get; set; }
    }
}
