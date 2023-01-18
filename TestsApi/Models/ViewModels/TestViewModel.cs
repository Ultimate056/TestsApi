namespace TestsApi.Models.ViewModels
{
    public class TestViewModel
    {
        public Test Test { get; set; }

        public List<Question> Questions { get; set; }

        public List<Answer> AllAnswers { get; set; }

        public TestViewModel(TestsContext db, Test test)
        {
            Questions = new List<Question>();
            AllAnswers = new List<Answer>();
            Test = test;
            Questions.AddRange(db.Questions.Where(u => u.TestId == test.Id).ToList());
            foreach(var q in Questions)
            {
                AllAnswers?.AddRange(db.Answers.Where(u => u.QuestionId == q.Id));
            }
        }
    }
}
