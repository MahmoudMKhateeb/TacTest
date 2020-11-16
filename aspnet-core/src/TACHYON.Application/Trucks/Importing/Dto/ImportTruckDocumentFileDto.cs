using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using TACHYON.Documents.DocumentFiles;

namespace TACHYON.Trucks.Importing.Dto
{
    public class ImportTruckDocumentFileDto : EntityDto<Guid>
    {
        [Required]
        [StringLength(DocumentFileConsts.MaxNameLength, MinimumLength = DocumentFileConsts.MinNameLength)]
        public virtual string Name { get; set; }

        [Required]
        [StringLength(DocumentFileConsts.MaxExtnLength, MinimumLength = DocumentFileConsts.MinExtnLength)]
        public virtual string Extn { get; set; }

        public DateTime? ExpirationDate { get; set; }

        public long DocumentTypeId { get; set; }

        public Guid? TruckId { get; set; }

        public string Number { get; set; }

        public string HijriExpirationDate { get; set; }
    }
}