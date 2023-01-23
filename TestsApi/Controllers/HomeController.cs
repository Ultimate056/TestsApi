using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TestsApi.Controllers
{
    public class HomeController : Controller
    {

        [HttpGet("/main")]
        [Authorize]
        public Task ViewMain()
        {
            HttpContext.Response.ContentType = "text/html; charset=utf-8";
            return HttpContext.Response.SendFileAsync("wwwroot/main.html");
        }
    }
}
