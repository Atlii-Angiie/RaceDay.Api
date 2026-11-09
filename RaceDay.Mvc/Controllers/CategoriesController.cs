using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using RaceDay.Mvc.Models;
namespace RaceDay.Mvc.Controllers;
public class CategoriesController:ApiMvcController {
 public CategoriesController(IHttpClientFactory f):base(f){}
 public async Task<IActionResult> Index(){if(Guard("Organiser") is {} denied)return denied;try{return View(await Client().GetFromJsonAsync<List<CategoryModel>>("api/Categories")??new());}catch(HttpRequestException){ViewBag.Error="API unavailable";return View(new List<CategoryModel>());}}
 [HttpPost,ValidateAntiForgeryToken] public async Task<IActionResult> Create(CategoryModel model){if(Guard("Organiser") is {} denied)return denied;if(!ModelState.IsValid){TempData["Notice"]="Category name required.";return RedirectToAction("Index");}var res=await Client().PostAsJsonAsync("api/Categories",new {model.Name,model.Description});TempData["Notice"]=res.IsSuccessStatusCode?"Category created.":await Error(res);return RedirectToAction("Index");}
 [HttpPost,ValidateAntiForgeryToken] public async Task<IActionResult> Delete(int id){if(Guard("Organiser") is {} denied)return denied;var res=await Client().DeleteAsync($"api/Categories/{id}");TempData["Notice"]=res.IsSuccessStatusCode?"Category deleted.":await Error(res);return RedirectToAction("Index");}
 [HttpPost,ValidateAntiForgeryToken] public async Task<IActionResult> Link(int categoryId,int eventId){if(Guard("Organiser") is {} denied)return denied;var res=await Client().PostAsync($"api/Categories/{categoryId}/events/{eventId}",null);TempData["Notice"]=res.IsSuccessStatusCode?"Category linked to event.":await Error(res);return RedirectToAction("Index");}
}
