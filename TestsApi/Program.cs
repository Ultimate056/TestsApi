using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Diagnostics.Metrics;
using System.Reflection.Metadata.Ecma335;
using TestsApi;
using TestsApi.Models;
using TestsApi.Models.ViewModels;


Result? result = null;

// Надо знать id пользователя
User? user;

var builder = WebApplication.CreateBuilder(args);

string? connection = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<TestsContext>(options => options.UseSqlite(connection));

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapGet("/test/{id}", (int id, TestsContext db, HttpContext context) =>
{
    var response = context.Response;
    response.Headers.ContentType = "application/json; charset=utf-8";
    var test = db.Tests.FirstOrDefault(u => u.Id == id);
    if (test == null) return Results.NotFound(new { message = "Тест не найден" });

    TestViewModel vm = new TestViewModel(db, test);

    var lastRes = db.Results.OrderBy(u=> u.Id).LastOrDefault();
    int counter = lastRes.Id;
    counter++;

    result = new Result { Id = counter, TestId = test.Id, Status = "Тест начался" };

    //db.Results.AddAsync(result);
    //db.SaveChangesAsync();

    return Results.Json(vm);
});

app.MapPost("/test", async (Answer[] answers, TestsContext db, HttpContext context) => {
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
    return Results.Json(answers);
});

app.MapGet("/results", async (TestsContext db) => await db.Results.ToListAsync());

app.MapGet("/results/{id:int}", async (int id, TestsContext db) =>
{
    Result? result = await db.Results.FirstOrDefaultAsync(u => u.Id == id);

    if (result == null) return Results.NotFound(new { message = "Результат не найден" });

    return Results.Json(result);
});

app.MapPut("/results", async (Result resultData, TestsContext db) =>
{
    Result? result = await db.Results.FirstOrDefaultAsync(u => u.Id == resultData.Id);
    if (result == null) return Results.NotFound(new { message = "Результат не найден" });

    result.ResultContent = resultData.ResultContent;
    result.Comment = resultData.Comment;
    result.Status = resultData.Status;

    await db.SaveChangesAsync();
    return Results.Json(result);
});

app.MapDelete("/results/{id:int}", async (int id, TestsContext db) =>
{
    Result? result = await db.Results.FirstOrDefaultAsync(u => u.Id == id);
    if (result == null) return Results.NotFound(new { message = "Результат не найден" });

    db.Results.Remove(result);
    await db.SaveChangesAsync();
    return Results.Json(result);
});


app.Run();
