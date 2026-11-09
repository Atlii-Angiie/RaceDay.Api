using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using RaceDay.Mvc.Models;
namespace RaceDay.Mvc.Controllers;
public class ParticipantController : ApiMvcController {
 public ParticipantController(IHttpClientFactory f):base(f){}
 public async Task<IActionResult> Index(){if(Guard("Participant") is {} denied)return denied;ViewBag.Name=HttpContext.Session.GetString("UserName");return View(await Load("api/Enrollments/me"));}
 private async Task<List<EnrolmentModel>> Load(string path){try{return await Client().GetFromJsonAsync<List<EnrolmentModel>>(path)??new();}catch(Exception ex) when(ex is HttpRequestException or System.Text.Json.JsonException){ViewBag.Error="Unable to load enrolments.";return new();}}
 [HttpGet] public async Task<IActionResult> Enrol(int id){if(Guard("Participant") is {} denied)return denied;ViewBag.EventId=id;try{ViewBag.Categories=await Client().GetFromJsonAsync<List<CategoryModel>>($"api/Categories/events/{id}")??new();}catch(Exception ex) when(ex is HttpRequestException or System.Text.Json.JsonException){ViewBag.Categories=new List<CategoryModel>();}return View();}
 [HttpPost,ValidateAntiForgeryToken] public async Task<IActionResult> Enrol(int eventId,int categoryId){if(Guard("Participant") is {} denied)return denied;var response=await Client().PostAsJsonAsync("api/Enrollments",new {eventId,categoryId});TempData["Notice"]=response.IsSuccessStatusCode?"Enrolment submitted.":await Error(response);return RedirectToAction("Index");}
 [HttpPost,ValidateAntiForgeryToken] public async Task<IActionResult> Cancel(int id){if(Guard("Participant") is {} denied)return denied;var response=await Client().DeleteAsync($"api/Enrollments/{id}");TempData["Notice"]=response.IsSuccessStatusCode?"Enrolment cancelled.":await Error(response);return RedirectToAction("Index");}
 public async Task<IActionResult> Results(){if(Guard("Participant") is {} denied)return denied;var response=await Client().GetAsync("api/Results/me");ViewBag.Content=response.IsSuccessStatusCode?await response.Content.ReadAsStringAsync():await Error(response);return View();}
 public async Task<IActionResult> Profile(){if(Guard() is {} denied)return denied;var response=await Client().GetAsync("api/Profiles/me");ViewBag.Content=response.IsSuccessStatusCode?await response.Content.ReadAsStringAsync():await Error(response);return View();}
}
