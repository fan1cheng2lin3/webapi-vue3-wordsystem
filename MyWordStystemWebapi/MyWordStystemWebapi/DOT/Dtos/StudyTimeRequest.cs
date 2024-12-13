using System.ComponentModel.DataAnnotations;

namespace MyWordStystemWebapi.DOT.Dtos
{
    public class StudyTimeRequest
    {
        [Key]
        public int UserId { get; set; } // 学习时长，单位为秒 


        public string? Adduptime { get; set; } // 学习时长，单位为秒

        
        public string? Day { get; set; } // 学习时长，单位为秒
    }
}
