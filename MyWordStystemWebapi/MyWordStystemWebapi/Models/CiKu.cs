using System.ComponentModel.DataAnnotations.Schema;

public class CiKuWord
{
    [Column("id")]
    public int id { get; set; }

    [Column("wordpre")]
    public string? wordpre { get; set; }

    [Column("phonetic")]
    public string? phonetic { get; set; }

    [Column("phonetic_uk")]
    public string? phonetic_uk { get; set; }

    [Column("explain")]
    public string? explain { get; set; }

    [Column("etyma")]
    public string? etyma { get; set; }

    [Column("sentence_en")]
    public string? sentence_en { get; set; }

    [Column("sentence_cn")]
    public string? sentence_cn { get; set; }

    [Column("ancillary")]
    public string? ancillary { get; set; }
}
