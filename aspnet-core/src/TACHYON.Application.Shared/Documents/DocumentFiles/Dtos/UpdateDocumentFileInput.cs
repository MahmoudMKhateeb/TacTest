using Abp.Extensions;
using Abp.Runtime.Validation;
using System;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.Documents.DocumentFiles.Dtos
{
    public class UpdateDocumentFileInput : ICustomValidate
    {
        [MaxLength(400)]
        public string FileToken { get; set; }


        public void AddValidationErrors(CustomValidationContext context)
        {
            if (FileToken.IsNullOrEmpty())
            {
                throw new ArgumentNullException(nameof(FileToken));
            }
        }
    }
}