using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Abp.Runtime.Validation;
using Abp.Extensions;

namespace TACHYON.AddressBook.Dtos
{
    public class CreateOrEditFacilityDto : EntityDto<long?>, ICustomValidate
    {
        [StringLength(FacilityConsts.MaxNameLength, MinimumLength = FacilityConsts.MinNameLength)]
        public string Name { get; set; }

        public string Address { get; set; }


        public double? Longitude { get; set; }

        public double? Latitude { get; set; }

        public int CityId { get; set; }
        public int? ShipperId { get; set; }
        public List<CreateOrEditFacilityWorkingHourDto> FacilityWorkingHours { get; set; }
        public int? ShipperActorId { get; set; }
        public bool IsForHomeDelivery { get; set; }
        public FacilityType FacilityType { get; set; }


        public void AddValidationErrors(CustomValidationContext context)
        {
            if(!IsForHomeDelivery && (Longitude==null || Latitude == null || Name.IsNullOrEmpty() || Address == null))
            {
                context.Results.Add(new ValidationResult("LocationAndNameAreRequired"));
            }

            if (!IsForHomeDelivery && (FacilityWorkingHours == null || FacilityWorkingHours.Count == 0))
            {
                context.Results.Add(new ValidationResult("FacilityWorkingHoursIsRequired"));
            }

            else if(IsForHomeDelivery && (Longitude == null || Latitude == null || Address == null) && Name.IsNullOrEmpty()){
                context.Results.Add(new ValidationResult("LocationOrNameAreRequired"));
            }
        }
    }
}