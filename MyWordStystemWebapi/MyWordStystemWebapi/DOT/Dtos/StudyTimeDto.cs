using System.ComponentModel.DataAnnotations;

namespace MyWordStystemWebapi.DOT.Dtos
{
    public class StudyTimeDto
    {
        [Key]
        public string? Adduptime { get; set; } // 学习时长，单位为秒

    }
}
