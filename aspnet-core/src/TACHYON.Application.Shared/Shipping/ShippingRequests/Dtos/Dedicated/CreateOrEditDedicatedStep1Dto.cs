using Abp.Application.Services.Dto;
using Abp.Runtime.Validation;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TACHYON.Shipping.ShippingRequests.Dtos.Dedicated
{
    public class CreateOrEditDedicatedStep1Dto : CreateOrEditShippingRequestStep1BaseDto ,IShippingRequestDtoHaveOthersName, ICustomValidate
    {
        public DateTime RentalStartDate { get; set; }
        public DateTime RentalEndDate { get; set; }
        public TimeUnit RentalDurationUnit { get; set; }
        public int NumberOfTrucks { get; set; }
        public double ExpectedMileage { get; set; }
        public string ServiceAreaNotes { get; set; }
        /// <summary>
        /// Number of duration per time unit
        /// </summary>
        [Required]
        [Range(1,100)]
        public int RentalDuration { get; set; }
        [Required] public int? GoodCategoryId { get; set; }
        public int PackingTypeId { get; set; }
        public virtual int? TransportTypeId { get; set; }
        public virtual long TrucksTypeId { get; set; }
        public virtual int? CapacityId { get; set; }
        public string OtherGoodsCategoryName { get; set; }
        public string OtherTransportTypeName { get; set; }
        public string OtherTrucksTypeName { get; set; }
        public string OtherPackingTypeName { get; set; }
        public int CountryId { get; set; }
        public List<ShippingRequestDestinationCitiesDto> ShippingRequestDestinationCities { get; set; }

        public void AddValidationErrors(CustomValidationContext context)
        {
            if (RentalStartDate.Date > RentalEndDate.Date)
            {
                context.Results.Add(new ValidationResult("The start date must be or equal to end date."));
            }

            if (IsBid)
            {
                RequestType = ShippingRequestType.Marketplace;
            }
            else if (IsTachyonDeal)
            {
                RequestType = ShippingRequestType.TachyonManageService;
            }
            else
            {
                RequestType = ShippingRequestType.DirectRequest;
            }
        }
    }
}
