using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using TACHYON.Common;
using TACHYON.CustomValidation;
using TACHYON.Routs.RoutPoints.Dtos;
namespace TACHYON.Shipping.Trips.Accidents.Dto
{
    public class CreateOrEditShippingRequestTripAccidentDto : EntityDto, IDocumentUpload
    {
        public int? TripId { get; set; }
        public int ReasoneId { get; set; }
        public string OtherReasonName { get; set; }
        [StringLength(500)]
        public string Description { get; set; }
        public Guid? DocumentId { get; set; }
        public string DocumentName { get; set; }
        public string DocumentContentType { get; set; }
        [UploadBase64File(MaxLength = 1048576 * 100)]
        public string DocumentBase64 { get; set; }
        public double? lat { get; set; }
        public double? lng { get; set; }
    }
}