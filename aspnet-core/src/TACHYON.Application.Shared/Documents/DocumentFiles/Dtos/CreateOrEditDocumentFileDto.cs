
using Abp.Application.Services.Dto;
using System;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.Documents.DocumentFiles.Dtos
{
    public class CreateOrEditDocumentFileDto : EntityDto<Guid?>
    {

        [Required]
        [StringLength(DocumentFileConsts.MaxNameLength, MinimumLength = DocumentFileConsts.MinNameLength)]
        public string Name { get; set; }


        [Required]
        [StringLength(DocumentFileConsts.MaxExtnLength, MinimumLength = DocumentFileConsts.MinExtnLength)]
        public string Extn { get; set; }


        public Guid BinaryObjectId { get; set; }


        public DateTime ExpirationDate { get; set; }


        public string IsAccepted { get; set; }


        public long DocumentTypeId { get; set; }

        public Guid? TruckId { get; set; }

        public long? TrailerId { get; set; }

        public long? UserId { get; set; }

        public long? RoutStepId { get; set; }

        public UpdateDocumentFileInput UpdateDocumentFileInput { get; set; }


    }
}