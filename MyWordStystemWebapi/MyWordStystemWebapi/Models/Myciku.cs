using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyWordStystemWebapi.Models
{
    public class Myciku
    {

        public int UserId { get; set; }
        public int WordId { get; set; }

        public string? wordpre { get; set; }

        public string? phonetic { get; set; }

        public string? phonetic_uk { get; set; }

        public string? explain { get; set; }

        public string? etyma { get; set; }

        public string? sentence_en { get; set; }

        public string? sentence_cn { get; set; }
        public string? WordBookName { get; set; }
        public string? Status { get; set; }

        public string? similar1 { get; set; }

        public string? similar2 { get; set; }

        public string? similar3 { get; set; }

        public string? similar4 { get; set; }

    
        public string? similar1_explain { get; set; }

    
        public string? similar2_explain { get; set; }

   
        public string? similar3_explain { get; set; }

   
        public string? similar4_explain { get; set; }

    }
}
