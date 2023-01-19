using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Diagnostics.Metrics;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;
using TestsApi;
using TestsApi.Models;
using TestsApi.Models.ViewModels;


Result? result = null;

// Надо знать id пользователя
User? user;

var builder = WebApplication.CreateBuilder(args);

string? connection = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<TestsContext>(options => options.UseSqlite(connection));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options => options.LoginPath = "/login");
builder.Services.AddAuthorization();


var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapGet("/", () =>
{
    return Results.Redirect("/login");
});

app.MapGet("/login", async (HttpContext context) =>
{
    context.Response.ContentType = "text/html; charset=utf-8";
    await context.Response.SendFileAsync("wwwroot/authorization.html");
});

app.MapPost("/login", async (string? returnUrl, HttpContext context, TestsContext db) =>
{
    var form = context.Request.Form;

    if (!form.ContainsKey("Login") || !form.ContainsKey("Password"))
        return Results.BadRequest("Логин и/или пароль не установлены");

    string login = form["Login"];
    string password = form["Password"];

    User? user = db.Users.FirstOrDefault(u => u.Login == login && u.Password == password);

    if (user is null) return Results.Unauthorized();

    var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())};
    ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "Cookies");
    await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

    context.Response.Cookies.Append("IdUser", user.Id.ToString());

    return Results.Redirect("/main");
});

app.MapGet("/logout", async (HttpContext context) =>
{
    await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return Results.Redirect("/login");
});

app.MapGet("/registration", async (HttpContext context) =>
{
    context.Response.ContentType = "text/html; charset=utf-8";
    await context.Response.SendFileAsync("wwwroot/registration.html");
});
app.MapPost("/registration", async (HttpContext context, TestsContext db) =>
{
    var form = context.Request.Form;

    string login = form["Login"];
    string password = form["Password"];
    string name = form["Name"];

    await db.Users.AddAsync(new User(login, password, name));
    await db.SaveChangesAsync();

    return Results.Redirect("/login");
});





// AuthorizedUser

app.MapGet("/main", [Authorize] async (HttpContext context) => {
    context.Response.ContentType = "text/html; charset=utf-8";
    await context.Response.SendFileAsync("wwwroot/main.html");
});


// TestController
app.MapGet("/test/{id}", [Authorize] (int id, TestsContext db, HttpContext context) =>
{
    var response = context.Response;
    response.Headers.ContentType = "application/json; charset=utf-8";
    var test = db.Tests.FirstOrDefault(u => u.Id == id);
    if (test == null) return Results.NotFound(new { message = "Тест не найден" });

    TestViewModel vm = new TestViewModel(db, test);

    var lastRes = db.Results.OrderBy(u=> u.Id).LastOrDefault();
    int counter = lastRes.Id;
    counter++;
    
    var user = db.Users.FirstOrDefault(u => u.Id == int.Parse(context.Request.Cookies["IdUser"]));


    result = new Result { Id = counter, UserId=user.Id, TestId = test.Id, Status = "Тест начался" };

    //db.Results.AddAsync(result);
    //db.SaveChangesAsync();

    return Results.Json(vm);
});

app.MapPost("/test", [Authorize]  async (Answer[] answers, TestsContext db, HttpContext context) => {
    var response = context.Response;
    response.Headers.ContentType = "application/json; charset=utf-8";
    // Логика работы с ответами
    // Алгоритмы определения личности
    // Инициализация свойств "Result"

    if (result != null)
    {
        result.ClosedTestTime = DateTime.Now;
        result.Status = "Тест завершён";
        result.Comment = Guid.NewGuid().ToString();

        await db.Results.AddAsync(result);
        await db.SaveChangesAsync();
    }
    return Results.Json(result);
});

//ResultController
app.MapGet("/results", [Authorize] async (TestsContext db) => await db.Results.ToListAsync());

app.MapGet("/results/{id:int}", [Authorize] async (int id, TestsContext db) =>
{
    Result? result = await db.Results.FirstOrDefaultAsync(u => u.Id == id);

    if (result == null) return Results.NotFound(new { message = "Результат не найден" });

    return Results.Json(result);
});

app.MapPut("/results", [Authorize] async (Result resultData, TestsContext db) =>
{
    Result? result = await db.Results.FirstOrDefaultAsync(u => u.Id == resultData.Id);
    if (result == null) return Results.NotFound(new { message = "Результат не найден" });

    result.ResultContent = resultData.ResultContent;
    result.Comment = resultData.Comment;
    result.Status = resultData.Status;

    await db.SaveChangesAsync();
    return Results.Json(result);
});

app.MapDelete("/results/{id:int}", [Authorize] async (int id, TestsContext db) =>
{
    Result? result = await db.Results.FirstOrDefaultAsync(u => u.Id == id);
    if (result == null) return Results.NotFound(new { message = "Результат не найден" });

    db.Results.Remove(result);
    await db.SaveChangesAsync();
    return Results.Json(result);
});


app.Run();
