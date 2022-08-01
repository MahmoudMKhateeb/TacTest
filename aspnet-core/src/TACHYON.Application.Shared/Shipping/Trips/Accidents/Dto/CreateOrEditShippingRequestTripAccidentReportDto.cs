using Abp.Application.Services.Dto;
using System;
using System.ComponentModel.DataAnnotations;
using TACHYON.Common;
using TACHYON.CustomValidation;

namespace TACHYON.Shipping.Trips.Accidents.Dto
{
    public class CreateOrEditShippingRequestTripAccidentReportDto : EntityDto<int?>, IDocumentUpload
    {
        public int AccidentId { get; set; }

        [StringLength(500, MinimumLength = 10)]
        public string Description { get; set; }

        [Required]
        public TripAccidentResolveType ResolveType { get; set; }
        public Guid? DocumentId { get; set; }
        public string DocumentName { get; set; }
        public string DocumentContentType { get; set; }

        [UploadBase64File(MaxLength = 1048576 * 100)]
        public string DocumentBase64 { get; set; }
    }
}