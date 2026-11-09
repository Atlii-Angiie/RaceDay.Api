using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using RaceDay.Mvc.Models;
namespace RaceDay.Mvc.Controllers;
public class EventsController : ApiMvcController {
 public EventsController(IHttpClientFactory f):base(f){}
 public async Task<IActionResult> Index(string? search) {
  if(Guard() is { } denied)return denied;
  try {var items=await Client().GetFromJsonAsync<List<RaceEvent>>("api/Events")??new();
   if(!string.IsNullOrWhiteSpace(search))items=items.Where(x=>x.Name.Contains(search,StringComparison.OrdinalIgnoreCase)||x.Location.Contains(search,StringComparison.OrdinalIgnoreCase)).ToList();
   return View(items.OrderBy(x=>x.EventDate).ToList());}
  catch(Exception ex) when(ex is HttpRequestException or System.Text.Json.JsonException){ViewBag.Error="Unable to load events from API.";return View(new List<RaceEvent>());}
 }
 [HttpGet] public async Task<IActionResult> Details(int id){if(Guard() is {} denied)return denied;var response=await Client().GetAsync($"api/Events/{id}");if(!response.IsSuccessStatusCode)return NotFound();return View(await response.Content.ReadFromJsonAsync<RaceEvent>());}
 [HttpGet] public IActionResult Create(){if(Guard("Organiser") is {} denied)return denied;return View("Edit",new RaceEvent());}
 [HttpPost,ValidateAntiForgeryToken] public async Task<IActionResult> Create(RaceEvent input){if(Guard("Organiser") is {} denied)return denied;if(!ModelState.IsValid)return View("Edit",input);var response=await Client().PostAsJsonAsync("api/Events",input);if(!response.IsSuccessStatusCode){ModelState.AddModelError("",await Error(response));return View("Edit",input);}TempData["Notice"]="Event created.";return RedirectToAction("Index","Organiser");}
 [HttpGet] public async Task<IActionResult> Edit(int id){if(Guard("Organiser") is {} denied)return denied;var response=await Client().GetAsync($"api/Events/{id}");if(!response.IsSuccessStatusCode)return NotFound();var item=await response.Content.ReadFromJsonAsync<RaceEvent>();if(item?.OrganizerId!=HttpContext.Session.GetInt32("UserId"))return StatusCode(403);return View(item);}
 [HttpPost,ValidateAntiForgeryToken] public async Task<IActionResult> Edit(int id,RaceEvent input){if(Guard("Organiser") is {} denied)return denied;if(!ModelState.IsValid)return View(input);var response=await Client().PutAsJsonAsync($"api/Events/{id}",input);if(!response.IsSuccessStatusCode){ModelState.AddModelError("",await Error(response));return View(input);}TempData["Notice"]="Event updated.";return RedirectToAction("Index","Organiser");}
 [HttpPost,ValidateAntiForgeryToken] public async Task<IActionResult> Delete(int id){if(Guard("Organiser") is {} denied)return denied;var response=await Client().DeleteAsync($"api/Events/{id}");TempData["Notice"]=response.IsSuccessStatusCode?"Event deleted.":await Error(response);return RedirectToAction("Index","Organiser");}

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UploadImage(int id, IFormFile image)
    {
        if (Guard("Organiser") is { } denied)
            return denied;

        if (image == null || image.Length == 0)
        {
            TempData["Notice"] = "Please select an image.";
            return RedirectToAction("Edit", new { id });
        }

        if (image.Length > 5 * 1024 * 1024)
        {
            TempData["Notice"] = "Image must be 5 MB or smaller.";
            return RedirectToAction("Edit", new { id });
        }

        using var form = new MultipartFormDataContent();
        using var stream = image.OpenReadStream();
        using var fileContent = new StreamContent(stream);

        fileContent.Headers.ContentType =
            new System.Net.Http.Headers.MediaTypeHeaderValue(
                image.ContentType);

        form.Add(fileContent, "image", image.FileName);

        var response = await Client().PostAsync(
            $"api/events/{id}/image", form);

        TempData["Notice"] = response.IsSuccessStatusCode
            ? "Race image uploaded successfully!"
            : await Error(response);

        return RedirectToAction("Edit", new { id });
        } 
        [HttpGet]
        public async Task<IActionResult> Image(int id)
        {
            if (Guard() is { } denied)
                return denied;

            var response = await Client().GetAsync($"api/events/{id}/image");

            if (!response.IsSuccessStatusCode)
                return NotFound();

            var bytes = await response.Content.ReadAsByteArrayAsync();

            var contentType = response.Content.Headers.ContentType?.MediaType
                              ?? "image/jpeg";

            return File(bytes, contentType);
        }
    }

