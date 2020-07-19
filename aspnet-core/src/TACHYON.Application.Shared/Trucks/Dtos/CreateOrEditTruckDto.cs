
using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.Trucks.Dtos
{
    public class CreateOrEditTruckDto : EntityDto<Guid?>
    {

        [Required]
        [StringLength(TruckConsts.MaxPlateNumberLength, MinimumLength = TruckConsts.MinPlateNumberLength)]
        public string PlateNumber { get; set; }


        [Required]
        [StringLength(TruckConsts.MaxModelNameLength, MinimumLength = TruckConsts.MinModelNameLength)]
        public string ModelName { get; set; }


        [Required]
        [StringLength(TruckConsts.MaxModelYearLength, MinimumLength = TruckConsts.MinModelYearLength)]
        public string ModelYear { get; set; }


        [Required]
        [StringLength(TruckConsts.MaxLicenseNumberLength, MinimumLength = TruckConsts.MinLicenseNumberLength)]
        public string LicenseNumber { get; set; }


        public DateTime LicenseExpirationDate { get; set; }


        public bool IsAttachable { get; set; }


        [StringLength(TruckConsts.MaxNoteLength, MinimumLength = TruckConsts.MinNoteLength)]
        public string Note { get; set; }


        public Guid TrucksTypeId { get; set; }

        public Guid TruckStatusId { get; set; }

        public long? Driver1UserId { get; set; }

        public long? Driver2UserId { get; set; }

        public int? RentPrice { get; set; }

        public int? RentDuration { get; set; }


    }
}