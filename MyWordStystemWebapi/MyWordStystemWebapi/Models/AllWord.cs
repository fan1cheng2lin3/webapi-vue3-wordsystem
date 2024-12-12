﻿using System.ComponentModel.DataAnnotations.Schema;

namespace MyWordStystemWebapi.Models
{
    public class AllWord
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


        [Column("similar1")]
        public string? similar1 { get; set; }

        [Column("similar2")]
        public string? similar2 { get; set; }

        [Column("similar3")]
        public string? similar3 { get; set; }

        [Column("similar4")]
        public string? similar4 { get; set; }

    }
}
