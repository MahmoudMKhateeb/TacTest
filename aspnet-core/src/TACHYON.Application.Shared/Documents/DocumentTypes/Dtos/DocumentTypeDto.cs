using Abp.Application.Services.Dto;

namespace TACHYON.Documents.DocumentTypes.Dtos
{
    public class DocumentTypeDto : EntityDto<long>
    {
        /// <summary>
        ///     multiLingual field mapped from DocumentTypeTranslation.Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     from DocumentTypeTranslation.Language
        /// </summary>
        public string Language { get; set; }


        public string DisplayName { get; set; }

        public bool IsRequired { get; set; }

        public bool HasExpirationDate { get; set; }

        public string RequiredFrom { get; set; }

        public bool HasNumber { get; set; }

        public bool HasNotes { get; set; }

        public bool IsNumberUnique { get; set; }

        public bool HasSpecialConstant { get; set; }
        public int NumberMinDigits { get; set; }

        public int NumberMaxDigits { get; set; }
        public int ExpirationAlertDays { get; set; }
        public bool InActiveAccountExpired { get; set; }
        public int InActiveToleranceDays { get; set; }
    }
}