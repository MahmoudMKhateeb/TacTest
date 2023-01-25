using Abp.Application.Services.Dto;
using Abp.Extensions;
using Abp.Runtime.Validation;
using System;
using System.ComponentModel.DataAnnotations;
using TACHYON.Common;
using TACHYON.CustomValidation;

namespace TACHYON.Shipping.Trips.Accidents.Dto
{
    public class CreateOrEditShippingRequestTripAccidentResolveDto : EntityDto<int?>, ICustomValidate
    {
        public int AccidentId { get; set; }

        [StringLength(500, MinimumLength = 3)]
        public string Description { get; set; }

        [Required]
        public TripAccidentResolveType ResolveType { get; set; }
        public long? DriverId { get; set; }

        public long? TruckId { get; set; }
        public void AddValidationErrors(CustomValidationContext context)
        {
            switch (ResolveType)
            {
                case TripAccidentResolveType.ChangeDriver:
                    if (!DriverId.HasValue )
                        context.Results.Add(new ValidationResult("You must select driver"));
                    break;
                case TripAccidentResolveType.ChangeTruck:
                    if (!TruckId.HasValue)
                        context.Results.Add(new ValidationResult("You must select truck"));
                    break;
                case TripAccidentResolveType.ChangeDriverAndTruck:
                    if (!DriverId.HasValue || !TruckId.HasValue)
                        context.Results.Add(new ValidationResult("You must select driver and truck"));
                    break;
                case TripAccidentResolveType.NoActionNeeded :
                    break;
                case TripAccidentResolveType.CancelTrip:
                    break;
                case TripAccidentResolveType.ResolveWithoutAction:
                    if (Description.IsNullOrEmpty())
                        context.Results.Add(new ValidationResult("You must add description"));
                    break;
                default:
                    context.Results.Add(new ValidationResult("Resolve type not valid"));
                    break;
            }
        }
    }
}