using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TestsApi.Models;

namespace TestsApi.Controllers
{
    public class ARController : Controller
    {
        private TestsContext db;
        public ARController(TestsContext db)
        {
            this.db = db;
        }

        
        [HttpGet("/login")]
        public Task FormAut()
        {
            HttpContext.Response.ContentType = "text/html; charset=utf-8";
            return HttpContext.Response.SendFileAsync("wwwroot/authorization.html");
        }

        [HttpPost("/login")]
        public async Task<ActionResult> GetResultAut(string? returnUrl)
        {
            var form = HttpContext.Request.Form;

            string? login = form["Login"];
            string? password = form["Password"];

            User? user = db.Users.FirstOrDefault(u => u.Login == login && u.Password == password);

            if (user is null) return Unauthorized();

            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()) };
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "Cookies");
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

            HttpContext.Response.Cookies.Append("IdUser", user.Id.ToString());

            return Redirect("/main");
        }

        [HttpGet("/logout")]
        public async Task<ActionResult> Out()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Redirect("/login");
        }

        [HttpGet("/registration")]
        public Task FormReg()
        {
            HttpContext.Response.ContentType = "text/html; charset=utf-8";
            return HttpContext.Response.SendFileAsync("wwwroot/registration.html");
        }

        [HttpPost("/registration")]
        public async Task <IActionResult> GetResultReg()
        {
            var form = HttpContext.Request.Form;

            string? login = form["Login"];
            string? password = form["Password"];
            string? name = form["Name"];

            await db.Users.AddAsync(new User(login, password, name));
            await db.SaveChangesAsync();

            return Redirect("/login");
        }
    }
}
