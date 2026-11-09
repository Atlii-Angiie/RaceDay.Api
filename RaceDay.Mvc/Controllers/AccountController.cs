
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace RaceDay.Mvc.Controllers
{
    public class AccountController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public AccountController(
            IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(
            string email,
            string password)
        {
            if (string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password))
            {
                ViewBag.ErrorMessage =
                    "Please enter your email and password.";

                return View();
            }

            var client = _httpClientFactory.CreateClient(
                "RaceDayApi");

            try
            {
                var response = await client.PostAsJsonAsync(
                    "api/Auth/login",
                    new
                    {
                        email,
                        password
                    });

                if (!response.IsSuccessStatusCode)
                {
                    ViewBag.ErrorMessage =
                        $"Login failed. API returned: {(int)response.StatusCode} " +
                        response.ReasonPhrase;

                    return View();
                }
                var loginResult =
                    await response.Content
                        .ReadFromJsonAsync<LoginResponse>();

                if (loginResult == null ||
                    string.IsNullOrWhiteSpace(
                        loginResult.AccessToken) ||
                    string.IsNullOrWhiteSpace(
                        loginResult.Role))
                {
                    ViewBag.ErrorMessage =
                        "Login response was invalid.";

                    return View();
                }

                // Save authentication details in session.
                HttpContext.Session.SetString(
                    "AccessToken",
                    loginResult.AccessToken);

                HttpContext.Session.SetString(
                    "UserRole",
                    loginResult.Role);

                HttpContext.Session.SetString(
                    "UserName",
                    loginResult.Name ?? "");

                HttpContext.Session.SetString(
                    "UserEmail",
                    loginResult.Email ?? "");

                HttpContext.Session.SetInt32(
                    "UserId",
                    loginResult.Id);

                // Redirect based on role.
                if (loginResult.Role == "Organiser")
                {
                    return RedirectToAction(
                        "Index", "Organiser");
                }

                if (loginResult.Role == "Participant")
                {
                    return RedirectToAction(
                        "Index", "Participant");
                }

                return RedirectToAction(
                    "Index", "Home");
            }
            catch (HttpRequestException)
            {
                ViewBag.ErrorMessage =
                    "Cannot connect to RaceDay API.";

                return View();
            }
            catch (System.Text.Json.JsonException)
            {
                ViewBag.ErrorMessage =
                    "Unable to process the login response.";

                return View();
            }
        }

        [HttpGet]
        public IActionResult Register() => View(new RaceDay.Mvc.Models.RegistrationInput());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RaceDay.Mvc.Models.RegistrationInput input)
        {
            if (!ModelState.IsValid) return View(input);
            if (input.Role != "Organiser" && input.Role != "Participant")
            {
                ModelState.AddModelError("Role", "Choose Organiser or Participant.");
                return View(input);
            }
            try
            {
                var client = _httpClientFactory.CreateClient("RaceDayApi");
                var response = await client.PostAsJsonAsync("api/Auth/register", input);
                if (response.IsSuccessStatusCode)
                {
                    TempData["Notice"] = "Registration successful. Please log in.";
                    return RedirectToAction("Login");
                }
                ModelState.AddModelError("", $"Registration failed ({(int)response.StatusCode}): " + await response.Content.ReadAsStringAsync());
            }
            catch (HttpRequestException) { ModelState.AddModelError("", "Cannot connect to RaceDay API."); }
            return View(input);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            return RedirectToAction(
                "Login", "Account");
        }

        public class LoginResponse
        {
            [JsonPropertyName("accessToken")]
            public string? AccessToken { get; set; }

            [JsonPropertyName("tokenType")]
            public string? TokenType { get; set; }

            [JsonPropertyName("expiresIn")]
            public int ExpiresIn { get; set; }

            [JsonPropertyName("id")]
            public int Id { get; set; }

            [JsonPropertyName("name")]
            public string? Name { get; set; }

            [JsonPropertyName("email")]
            public string? Email { get; set; }

            [JsonPropertyName("role")]
            public string? Role { get; set; }
        }
    }
}
