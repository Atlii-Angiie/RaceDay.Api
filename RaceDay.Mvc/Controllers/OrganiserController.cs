
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace RaceDay.Mvc.Controllers
{
    public class OrganiserController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public OrganiserController(
            IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index()
        {
            var token = HttpContext.Session.GetString("AccessToken");
            var role = HttpContext.Session.GetString("UserRole");
            var userId = HttpContext.Session.GetInt32("UserId");

            if (string.IsNullOrWhiteSpace(token))
            {
                return RedirectToAction("Login", "Account");
            }

            if (role != "Organiser")
            {
                return StatusCode(403);
            }

            ViewBag.UserName =
                HttpContext.Session.GetString("UserName");

            var client = _httpClientFactory.CreateClient("RaceDayApi");

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            try
            {
                var response = await client.GetAsync("api/Events");

                if (response.StatusCode ==
                    System.Net.HttpStatusCode.Unauthorized)
                {
                    HttpContext.Session.Clear();
                    return RedirectToAction("Login", "Account");
                }

                if (!response.IsSuccessStatusCode)
                {
                    ViewBag.ErrorMessage = "Unable to load your events.";
                    return View(new List<EventSummary>());
                }

                var allEvents = await response.Content
                    .ReadFromJsonAsync<List<EventSummary>>()
                    ?? new List<EventSummary>();

                var myEvents = allEvents
                    .Where(e => e.OrganizerId == userId)
                    .OrderBy(e => e.EventDate)
                    .ToList();

                // Get enrolment counts for each event.
                foreach (var raceEvent in myEvents)
                {
                    try
                    {
                        var enrolmentResponse = await client.GetAsync(
                            $"api/Enrollments/event/{raceEvent.Id}");

                        if (enrolmentResponse.IsSuccessStatusCode)
                        {
                            var enrolments = await enrolmentResponse.Content
                                .ReadFromJsonAsync<List<EnrolmentSummary>>();

                            raceEvent.TotalEnrolments =
                                enrolments?.Count ?? 0;
                        }
                        else
                        {
                            raceEvent.TotalEnrolments = null;
                        }
                    }
                    catch (HttpRequestException)
                    {
                        raceEvent.TotalEnrolments = null;
                    }
                    catch (System.Text.Json.JsonException)
                    {
                        raceEvent.TotalEnrolments = null;
                    }
                }

                return View(myEvents);
            }
            catch (HttpRequestException)
            {
                ViewBag.ErrorMessage = "Cannot connect to RaceDay API.";
                return View(new List<EventSummary>());
            }
            catch (System.Text.Json.JsonException)
            {
                ViewBag.ErrorMessage = "Unable to read event information.";
                return View(new List<EventSummary>());
            }
        }

        public class EventSummary
        {
            public int Id { get; set; }
            public string Name { get; set; } = "";
            public DateTime EventDate { get; set; }
            public string Location { get; set; } = "";
            public decimal Distance { get; set; }
            public decimal EntryFee { get; set; }
            public int OrganizerId { get; set; }
            public string Status { get; set; } = "";
            public int? TotalEnrolments { get; set; }
        }

        public class EnrolmentSummary
        {
            public int Id { get; set; }
            public int EventId { get; set; }
            public int UserId { get; set; }
            public int CategoryId { get; set; }
            public DateTime RegistrationDate { get; set; }
            public string? RaceNumber { get; set; }
            public string Status { get; set; } = "";
        }
    }
}
