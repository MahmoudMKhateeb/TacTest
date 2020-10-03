using System.ComponentModel.DataAnnotations;

namespace TACHYON.Documents.DocumentTypes.Dtos
{
    public class DocumentTypeTranslationDto
    {
        [Required]
        [StringLength(DocumentTypeConsts.MaxDisplayNameLength, MinimumLength = DocumentTypeConsts.MinDisplayNameLength)]
        public virtual string Name { get; set; }

        public string Language { get; set; }
        public long CoreId { get; set; }
    }
}