using System.ComponentModel.DataAnnotations;
using Translation.Data.Entities;

namespace Translation.Api.Models
{
    public class TranslationItemDto
    {
        [Required]
        [MaxLength(100)]
        public string Key { get; set; }

        [Required]
        public string Value { get; set; }


        public static TranslationItemDto Construct(TranslationItem translationItem)
        {
            return new TranslationItemDto
            {
                Key = translationItem.Key,
                Value = translationItem.Value
            };
        }

        public static TranslationItem Deconstruct(TranslationItemDto dto)
        {
            return new TranslationItem
            {
                Key = dto.Key,
                Value = dto.Value
            };
        }
    }
}