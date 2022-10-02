using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TACHYON.Documents.DocumentFiles.Dtos;

namespace TACHYON.Trucks.Dtos
{
    public class CreateOrEditTruckDto : EntityDto<long?>
    {
        [Required]
        [StringLength(TruckConsts.MaxPlateNumberLength, MinimumLength = TruckConsts.MinPlateNumberLength)]
        [RegularExpression(TruckConsts.PlateNumberRegularExpression)]
        public string PlateNumber { get; set; }


        [Required]
        [StringLength(TruckConsts.MaxModelNameLength, MinimumLength = TruckConsts.MinModelNameLength)]
        public string ModelName { get; set; }


        [Required]
        [StringLength(TruckConsts.MaxModelYearLength, MinimumLength = TruckConsts.MinModelYearLength)]
        [RegularExpression(TruckConsts.ModelYearRegularExpression)]
        public string ModelYear { get; set; }


        public virtual string Capacity { get; set; }
        public bool IsAttachable { get; set; }


        [StringLength(TruckConsts.MaxNoteLength, MinimumLength = TruckConsts.MinNoteLength)]
        public string Note { get; set; }

        [Required] public long? TruckStatusId { get; set; }


        public UpdateTruckPictureInput UpdateTruckPictureInput { get; set; }

        public List<CreateOrEditDocumentFileDto> CreateOrEditDocumentFileDtos { get; set; }

        #region Truck Categories

        public virtual int? TransportTypeId { get; set; }
        public virtual long? TrucksTypeId { get; set; }
        public virtual int? CapacityId { get; set; }

        #endregion

        public int? Length { get; set; }

        [Required] public virtual int PlateTypeId { get; set; }

        public int? TenantId { get; set; }

        public string OtherTrucksTypeName { get; set; }
        public string OtherTransportTypeName { get; set; }
        public int? CarrierActorId { get; set; }


        [StringLength(TruckConsts.MaxInternalTruckIdLength)]
        public string InternalTruckId { get; set; }

    }
}