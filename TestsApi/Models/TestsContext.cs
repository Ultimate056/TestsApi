using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace TestsApi.Models
{
    public class TestsContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public DbSet<Test> Tests { get; set; }

        public DbSet<Question> Questions { get; set; }

        public DbSet<Answer> Answers { get; set; }

        public DbSet<Result> Results { get; set; }

        public TestsContext(DbContextOptions<TestsContext> options) : base(options) {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            DateTime now = DateTime.Now;
            Test testA = new Test { Id = 1, Name = "Тест А", Description = "AAAAAA", createdDate = now };

            modelBuilder.Entity<User>().HasData(
                    new User { Id = 1, Login="tommi", Password="123", Name = "Tom" },
                    new User { Id = 2, Login="bobbi", Password="123", Name = "Bob" },
                    new User { Id = 3, Login="sammi" , Password="123", Name = "Sam"}
            );
            
            modelBuilder.Entity<Test>().HasData(testA);

            Question q1forA = new Question { Id = 1, Content = "Вопрос 1 на тест А.", TestId = testA.Id };  
            Question q2forA = new Question { Id = 2, Content = "Вопрос 2 на тест А.", TestId = testA.Id };
            Question q3forA = new Question { Id = 3, Content = "Вопрос 3 на тест А.", TestId = testA.Id };
          
            modelBuilder.Entity<Question>().HasData(q1forA, q2forA, q3forA);

            Answer A1forQ1 = new Answer { Id = 1, Content = "Ответ 1 на вопрос 1", QuestionId = q1forA.Id };
            Answer A2forQ1 = new Answer { Id = 2, Content = "Ответ 2 на вопрос 1", QuestionId = q1forA.Id };
            Answer A3forQ1 = new Answer { Id = 3, Content = "Ответ 3 на вопрос 1", QuestionId = q1forA.Id };

            Answer A1forQ2 = new Answer { Id = 4, Content = "Ответ 1 на вопрос 2", QuestionId = q2forA.Id };
            Answer A2forQ2 = new Answer { Id = 5, Content = "Ответ 2 на вопрос 2", QuestionId = q2forA.Id };
            Answer A3forQ2 = new Answer { Id = 6, Content = "Ответ 3 на вопрос 2", QuestionId = q2forA.Id };

            Answer A1forQ3 = new Answer { Id = 7, Content = "Ответ 1 на вопрос 3", QuestionId = q3forA.Id };
            Answer A2forQ3 = new Answer { Id = 8, Content = "Ответ 2 на вопрос 3", QuestionId = q3forA.Id };
            Answer A3forQ3 = new Answer { Id = 9, Content = "Ответ 3 на вопрос 3", QuestionId = q3forA.Id };

           
            modelBuilder.Entity<Answer>().HasData(A1forQ1, A2forQ1, A3forQ1, A1forQ2, A2forQ2, A3forQ2,
                A1forQ3, A2forQ3, A3forQ3);
        }
    }
}
