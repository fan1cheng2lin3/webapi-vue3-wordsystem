using System.ComponentModel.DataAnnotations;

namespace MyWordStystemWebapi.DOT.Dtos
{
    public class StartwordDto
    {

        [Key]
        public int UserId { get; set; }
        public int WordId { get; set; }


        public string? collect { get; set; }
    }
}
