namespace MyWordStystemWebapi.Models
{
    public class WordBooks
    {

        public int Id { get; set; }
        public int UserId { get; set; }
        public int? WordId { get; set; }

        public string? WordBookName { get; set; }
    

    }
}
