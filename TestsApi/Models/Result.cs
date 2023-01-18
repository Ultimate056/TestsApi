namespace TestsApi.Models
{
    public class Result
    {
        public int Id { get; set; }

        public int TestId { get; set; } 

        public string? ResultContent { get; set; }  

        public int UserId { get; set; }

        public DateTime? ClosedTestTime { get; set; }   

        public string? Comment { get; set; }
        public string? Status { get; set; }


    }
}
