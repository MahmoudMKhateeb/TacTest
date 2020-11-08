
using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TACHYON.Documents.DocumentFiles.Dtos;

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


        //[Required]
        //[StringLength(TruckConsts.MaxLicenseNumberLength, MinimumLength = TruckConsts.MinLicenseNumberLength)]
        //public string LicenseNumber { get; set; }


        //public DateTime LicenseExpirationDate { get; set; }


        public bool IsAttachable { get; set; }


        [StringLength(TruckConsts.MaxNoteLength, MinimumLength = TruckConsts.MinNoteLength)]
        public string Note { get; set; }

        public long? TruckStatusId { get; set; }

        public long? Driver1UserId { get; set; }

        //public long? Driver2UserId { get; set; }

        //public int? RentPrice { get; set; }

        //public int? RentDuration { get; set; }

        public UpdateTruckPictureInput UpdateTruckPictureInput { get; set; }

        public List<CreateOrEditDocumentFileDto> CreateOrEditDocumentFileDtos { get; set; }

        #region Truck Categories

        public virtual int? TransportTypeId { get; set; }


        public virtual int? TransportSubtypeId { get; set; }

        public virtual long? TrucksTypeId { get; set; }


        public virtual int? TruckSubtypeId { get; set; }


        public virtual int? CapacityId { get; set; }

        #endregion

    }
}