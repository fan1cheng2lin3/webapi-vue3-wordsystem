using System.ComponentModel.DataAnnotations;

namespace MyWordStystemWebapi.Models
{
    public class Studydate_Table
    {
        [Key] // 确保此字段是主键
        public int UserId { get; set; }

        [Required]
        [MaxLength(10)]
        public string Day { get; set; } // 确保此字段存储格式为 yyyy-MM-dd

        [Required]
        public string? Adduptime { get; set; }
    }

}
