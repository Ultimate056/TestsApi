using Microsoft.AspNetCore.Mvc;
using TestsApi.Models.ViewModels;
using TestsApi.Models;
using Microsoft.AspNetCore.Authorization;

namespace TestsApi.Controllers
{
    [ApiController]
    [Route("/{Controller}")]
    [Authorize]
    public class TestController : Controller
    {

        private TestsContext db;

        private static Result? resultTest = null;

        public TestController(TestsContext db)
        {
            this.db = db;
        }

        [HttpGet("{id}")]
        [Authorize]
        public IActionResult GetTest(int id)
        {
            var response = HttpContext.Response;
            response.Headers.ContentType = "application/json; charset=utf-8";

            // Находим тест из БД
            var test = db.Tests.FirstOrDefault(u => u.Id == id);
            if (test == null) return NotFound(new { message = "Тест не найден" });

            // Инициализируем вьюмодель
            TestViewModel vm = new TestViewModel(db, test);

            // Берётся значение максимального айдишника таблицы "Результаты"
            var lastRes = db.Results.OrderBy(u => u.Id).LastOrDefault();
            int counter = lastRes.Id;
            counter++;

            // Находим пользователя, проходившего тест
            var user = db.Users.FirstOrDefault(u => u.Id == int.Parse(HttpContext.Request.Cookies["IdUser"]));

            // Инициализируем объект результата
            resultTest = new Result { Id = counter, UserId = user.Id, TestId = test.Id, Status = "Тест начался" };

            //db.Results.AddAsync(result);
            //db.SaveChangesAsync();

            // Отправляем вьюмодель в виде json
            return Json(vm);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> GetResultsTest(Answer[] answers)
        {
            var response = HttpContext.Response;
            response.Headers.ContentType = "application/json; charset=utf-8";
            // Логика работы с ответами
            // Алгоритмы определения личности
            // Инициализация свойств "Result"

            if (resultTest != null)
            {
                resultTest.ClosedTestTime = DateTime.Now;
                resultTest.Status = "Тест завершён";
                resultTest.Comment = Guid.NewGuid().ToString();

                await db.Results.AddAsync(resultTest);
                await db.SaveChangesAsync();
            }
            return Json(resultTest);
        }

    }
}
