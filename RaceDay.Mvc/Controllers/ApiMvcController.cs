using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Net.Http.Json;
namespace RaceDay.Mvc.Controllers;
public abstract class ApiMvcController : Controller {
 protected readonly IHttpClientFactory Factory;
 protected ApiMvcController(IHttpClientFactory factory) => Factory=factory;
 protected HttpClient Client() {var c=Factory.CreateClient("RaceDayApi");var token=HttpContext.Session.GetString("AccessToken");if(!string.IsNullOrWhiteSpace(token))c.DefaultRequestHeaders.Authorization=new AuthenticationHeaderValue("Bearer",token);return c;}
 protected IActionResult? Guard(string? role=null) {
  if(string.IsNullOrEmpty(HttpContext.Session.GetString("AccessToken")))return RedirectToAction("Login","Account");
  if(role!=null && HttpContext.Session.GetString("UserRole")!=role)return StatusCode(403);
  return null;
 }
 protected async Task<string> Error(HttpResponseMessage response) {var detail=await response.Content.ReadAsStringAsync();return $"API returned {(int)response.StatusCode}: {detail[..Math.Min(detail.Length,350)]}";}
}
