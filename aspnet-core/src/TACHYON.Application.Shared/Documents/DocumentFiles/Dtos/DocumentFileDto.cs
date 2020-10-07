
using Abp.Application.Services.Dto;
using System;

namespace TACHYON.Documents.DocumentFiles.Dtos
{
    public class DocumentFileDto : EntityDto<Guid>
    {
        public string Name { get; set; }

        public string Extn { get; set; }

        public Guid BinaryObjectId { get; set; }

        public DateTime ExpirationDate { get; set; }

        public bool IsAccepted { get; set; }

        public long DocumentTypeId { get; set; }

        public Guid? TruckId { get; set; }

        public long? TrailerId { get; set; }

        public long? UserId { get; set; }

        public long? RoutStepId { get; set; }

        public int? Number { get; set; }
        public string Notes { get; set; }

        public string SpecialConstant { get; set; }

    }
}