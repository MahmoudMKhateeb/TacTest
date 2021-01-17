
using Abp.Application.Services.Dto;
using System;

namespace TACHYON.Trailers.Dtos
{
    public class TrailerDto : EntityDto<long>
    {
        public string TrailerCode { get; set; }

        public string PlateNumber { get; set; }

        public string Model { get; set; }

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