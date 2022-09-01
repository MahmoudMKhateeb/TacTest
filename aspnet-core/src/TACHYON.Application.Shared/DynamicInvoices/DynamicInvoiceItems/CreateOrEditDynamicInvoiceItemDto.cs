using Abp.Application.Services.Dto;
using Abp.Runtime.Validation;
using Castle.Core.Internal;
using System;
using System.ComponentModel.DataAnnotations;
using TACHYON.Trucks;

namespace TACHYON.DynamicInvoices.DynamicInvoiceItems
{
    public class CreateOrEditDynamicInvoiceItemDto : EntityDto<long?>, ICustomValidate
    {
        [Required]
        [StringLength(256,MinimumLength = 2)]
        public string Description { get; set; }
        
        [Required]
        public decimal Price { get; set; }

        public long? WaybillNumber { get; set; }

        public int? OriginCityId { get; set; }

        public int? DestinationCityId { get; set; }

        public long? TruckTypeId { get; set; }

        public DateTime? WorkDate { get; set; }

        [StringLength(TruckConsts.MaxPlateNumberLength, MinimumLength = TruckConsts.MinPlateNumberLength)]
        [RegularExpression(TruckConsts.PlateNumberRegularExpression)]
        public string PlateNumber { get; set; }

        public int? Quantity { get; set; }

        public string ContainerNumber { get; set; }

        public decimal VatAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public void AddValidationErrors(CustomValidationContext context)
        {
            if (WaybillNumber.HasValue && IsAnyFieldHasValue())
                context.Results.Add(new ValidationResult("YouCanNotManualEditFieldsWithWaybillNumber"));
        }

        private bool IsAnyFieldHasValue()
        {
            return !ContainerNumber.IsNullOrEmpty() || !PlateNumber.IsNullOrEmpty() || Quantity.HasValue
                   || TruckTypeId.HasValue || OriginCityId.HasValue || DestinationCityId.HasValue || WorkDate.HasValue;
        }
    }
}