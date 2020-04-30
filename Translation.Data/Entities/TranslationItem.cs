using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace Translation.Data.Entities
{
    public class TranslationItem
    {
        [Key]
        [Column(TypeName = "varchar(100)")]
        public string Key { get; set; }

        [NotNull]
        [Column(TypeName = "varchar(5)")]
        public string Language { get; set; }

        [NotNull]
        public string Value { get; set; }
    }
}
