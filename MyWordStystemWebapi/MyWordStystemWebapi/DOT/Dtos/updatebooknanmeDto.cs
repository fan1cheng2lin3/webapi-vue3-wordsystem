using System.ComponentModel.DataAnnotations;

namespace MyWordStystemWebapi.DOT.Dtos
{
    public class updatebooknanmeDto
    {

        [Key]
        public int Id { get; set; }


        public string? WordBookName { get; set; }

    }
}
