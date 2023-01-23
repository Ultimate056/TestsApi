using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestsApi.Models;

namespace TestsApi.Controllers
{
    [ApiController]
    [Route("/api/{controller}")]
    [Authorize]
    public class ResultsController : Controller
    {
        private TestsContext db;

        public ResultsController(TestsContext db)
        {
            this.db = db;
        }

        // Список результатов
        [HttpGet]
        public async Task<IList<Result>> GetResults()
        {
            return await db.Results.ToListAsync();
        }

        // Возвращаем один результат
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetResult(int id)
        {
            Result? result = await db.Results.FirstOrDefaultAsync(u => u.Id == id);

            if (result == null) return NotFound(new { message = "Результат не найден" });

            return Json(result);
        }

        // Изменение результата
        [HttpPut]
        public async Task<IActionResult> UpdateResult(Result resultData)
        {
            Result? result = await db.Results.FirstOrDefaultAsync(u => u.Id == resultData.Id);
            if (result == null) return NotFound(new { message = "Результат не найден" });

            result.ResultContent = resultData.ResultContent;
            result.Comment = resultData.Comment;
            result.Status = resultData.Status;

            await db.SaveChangesAsync();
            return Json(result);
        }

        // Удаление результата
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteResult(int id)
        {
            Result? result = await db.Results.FirstOrDefaultAsync(u => u.Id == id);
            if (result == null) return NotFound(new { message = "Результат не найден" });

            db.Results.Remove(result);
            await db.SaveChangesAsync();
            return Json(result);
        }



    }
}
