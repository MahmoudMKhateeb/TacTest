
using Abp.Application.Services.Dto;
using System;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.Trailers.Dtos
{
    public class CreateOrEditTrailerDto : EntityDto<long?>
    {

        [Required]
        [StringLength(TrailerConsts.MaxTrailerCodeLength, MinimumLength = TrailerConsts.MinTrailerCodeLength)]
        public string TrailerCode { get; set; }


        [Required]
        [StringLength(TrailerConsts.MaxPlateNumberLength, MinimumLength = TrailerConsts.MinPlateNumberLength)]
        public string PlateNumber { get; set; }


        [Required]
        [StringLength(TrailerConsts.MaxModelLength, MinimumLength = TrailerConsts.MinModelLength)]
        public string Model { get; set; }


        [Required]
        [StringLength(TrailerConsts.MaxYearLength, MinimumLength = TrailerConsts.MinYearLength)]
        public string Year { get; set; }


        public int Width { get; set; }


        public int Height { get; set; }


        public int Length { get; set; }


        public bool IsLiftgate { get; set; }


        public bool IsReefer { get; set; }


        public bool IsVented { get; set; }


        public bool IsRollDoor { get; set; }


        public int TrailerStatusId { get; set; }

        public int TrailerTypeId { get; set; }

        public int PayloadMaxWeightId { get; set; }

        public long? HookedTruckId { get; set; }


    }
}