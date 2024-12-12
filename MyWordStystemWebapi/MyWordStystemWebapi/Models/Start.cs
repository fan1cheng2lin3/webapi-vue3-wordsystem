using System.ComponentModel.DataAnnotations;

namespace MyWordStystemWebapi.Models
{
    public class Start
    {
        [Key]
        public int Id { get; set; }

        public int UserId { get; set; }

        public int WordId { get; set; }

        public string? Status { get; set; }

    }
}
