using System.ComponentModel.DataAnnotations;

namespace MyWordStystemWebapi.DOT.Dtos
{
    public class addwordbookDto
    {

        [Key]
        public int UserId { get; set; }

        public string? WordBookName { get; set; }

    }
}
