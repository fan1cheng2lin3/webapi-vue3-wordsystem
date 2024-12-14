namespace MyWordStystemWebapi.Models
{
    public class LearningProgress
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int WordId { get; set; }
        public int count { get; set; }
        public int reScore { get; set; }
        public int Score { get; set; }
        public string? lasttime { get; set; }
        public string? nextxuexi { get; set; }
        public string? Status { get; set; }


    }
}
