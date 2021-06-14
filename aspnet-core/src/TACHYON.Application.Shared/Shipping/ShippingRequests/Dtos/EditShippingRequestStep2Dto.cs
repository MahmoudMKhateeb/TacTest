using Abp.Application.Services.Dto;
using Abp.Runtime.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TACHYON.Shipping.ShippingRequests.Dtos
{
    public class EditShippingRequestStep2Dto : EntityDto<long>, ICustomValidate
    {
        [Required]
        public ShippingRequestRouteType RouteTypeId { get; set; }

        [Required]
        public int OriginCityId { get; set; }
        [Required]
        public int DestinationCityId { get; set; }
        [Required]
        public int NumberOfDrops { get; set; }
        [Required]
        public int NumberOfTrips { get; set; }

        public void AddValidationErrors(CustomValidationContext context)
        {
            switch (this.RouteTypeId)
            {
                case ShippingRequestRouteType.SingleDrop:
                    this.NumberOfDrops = 1;
                    break;
                case ShippingRequestRouteType.TwoWay:
                    this.NumberOfDrops = 2;
                    break;
                default:
                    if (this.NumberOfDrops < 2)
                        context.Results.Add(new ValidationResult("TheNumberOfDropsMustHigerOrEqualTwo"));
                    break;
            }
        }
    }
}
