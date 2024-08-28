using Abp.Application.Services.Dto;
using Abp.Extensions;
using Abp.Runtime.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TACHYON.Documents.DocumentFiles.Dtos;
using TACHYON.Integration.BayanIntegration;

namespace TACHYON.Trucks.Dtos
{
    public class CreateOrEditTruckDto : EntityDto<long?>, ICanBeExcludedFromBayanIntegration, ICustomValidate, IShouldNormalize
    {


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
        public virtual long? DriverUserId { get; set; }

        public int? CarrierActorId { get; set; }


        [StringLength(TruckConsts.MaxInternalTruckIdLength)]
        public string InternalTruckId { get; set; }

        public bool ExcludeFromBayanIntegration { get; set; }

        public string EquipNumber { get; set; } //SAB

        public PlateNumberDto PlateNumberDto { get; set; }

        public void AddValidationErrors(CustomValidationContext context)
        {
            if (PlateNumber.IsNullOrEmpty() && PlateNumberDto == null)
            {
                context.Results.Add(new ValidationResult("The PlateNumber or PlateNumberDto should have a value."));
            }
        }

        public void Normalize()
        {
            if ( PlateNumberDto != null)
            {
                PlateNumber = PlateNumberDto.GeneratePlateNumber();
            }
            else if (!PlateNumber.IsNullOrEmpty() && PlateNumberDto == null)
            {
                var cleanedPlateNumber = PlateNumber.Replace(" ", "");
                PlateNumberDto = new PlateNumberDto
                {
                    FirstNumber = cleanedPlateNumber[0].ToString(),
                    SecondNumber = cleanedPlateNumber[1].ToString(),
                    ThirdNumber = cleanedPlateNumber[2].ToString(),
                    FourthNumber = cleanedPlateNumber[3].ToString(),
                    FirstChar = cleanedPlateNumber[4].ToString(),
                    SecondChar = cleanedPlateNumber[5].ToString(),
                    ThirdChar = cleanedPlateNumber[6].ToString()
                };
            }
        }

    }



    public class PlateNumberDto
    {
        [Required]
        [StringLength(1, ErrorMessage = TruckConsts.ExactlyOneCharacterLong)]
        [RegularExpression(@"^\d$", ErrorMessage = TruckConsts.NumericCharacter)]
        public string FirstNumber { get; set; }

        [Required]
        [StringLength(1, ErrorMessage = TruckConsts.ExactlyOneCharacterLong)]
        [RegularExpression(@"^\d$", ErrorMessage = TruckConsts.NumericCharacter)]
        public string SecondNumber { get; set; }

        [Required]
        [StringLength(1, ErrorMessage = TruckConsts.ExactlyOneCharacterLong)]
        [RegularExpression(@"^\d$", ErrorMessage = TruckConsts.NumericCharacter)]
        public string ThirdNumber { get; set; }

        [Required]
        [StringLength(1, ErrorMessage = TruckConsts.ExactlyOneCharacterLong)]
        [RegularExpression(@"^\d$", ErrorMessage = TruckConsts.NumericCharacter)]
        public string FourthNumber { get; set; }

        [Required]
        [StringLength(1, ErrorMessage = TruckConsts.ExactlyOneCharacterLong)]
        public string FirstChar { get; set; }

        [Required]
        [StringLength(1, ErrorMessage = TruckConsts.ExactlyOneCharacterLong)]
        public string SecondChar { get; set; }

        [Required]
        [StringLength(1, ErrorMessage = TruckConsts.ExactlyOneCharacterLong)]
        public string ThirdChar { get; set; }

        public string GeneratePlateNumber()
        {
            return $"{FirstNumber}{SecondNumber}{ThirdNumber}{FourthNumber} {FirstChar} {SecondChar} {ThirdChar}";
        }
    }
}

