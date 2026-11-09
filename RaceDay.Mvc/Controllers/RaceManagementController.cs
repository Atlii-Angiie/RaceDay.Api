using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using RaceDay.Mvc.Models;
namespace RaceDay.Mvc.Controllers;
public class RaceManagementController:ApiMvcController {
 public RaceManagementController(IHttpClientFactory f):base(f){}
 public async Task<IActionResult> Enrolments(int id){if(Guard("Organiser") is {} denied)return denied;ViewBag.EventId=id;var res=await Client().GetAsync($"api/Enrollments/event/{id}");if(!res.IsSuccessStatusCode){ViewBag.Error=await Error(res);return View(new List<EnrolmentModel>());}return View(await res.Content.ReadFromJsonAsync<List<EnrolmentModel>>()??new());}
 public async Task<IActionResult> Results(int id){if(Guard("Organiser") is {} denied)return denied;ViewBag.EventId=id;var res=await Client().GetAsync($"api/Results/event/{id}");ViewBag.Content=res.IsSuccessStatusCode?await res.Content.ReadAsStringAsync():await Error(res);return View();}
 [HttpGet] public IActionResult RecordResult(int id){if(Guard("Organiser") is {} denied)return denied;ViewBag.EventId=id;return View(new ResultInput());}
 [HttpPost,ValidateAntiForgeryToken] public async Task<IActionResult> RecordResult(int id,ResultInput input){if(Guard("Organiser") is {} denied)return denied;if(!ModelState.IsValid){ViewBag.EventId=id;return View(input);}var res=await Client().PostAsJsonAsync("api/Results",input);if(!res.IsSuccessStatusCode){ModelState.AddModelError("",await Error(res));ViewBag.EventId=id;return View(input);}TempData["Notice"]="Result saved.";return RedirectToAction("Results",new{id});}
}
