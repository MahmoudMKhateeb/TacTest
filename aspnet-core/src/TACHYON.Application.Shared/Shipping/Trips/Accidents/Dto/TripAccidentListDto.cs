using Abp.Application.Services.Dto;
using System;

namespace TACHYON.Shipping.Trips.Accidents.Dto
{
    public class TripAccidentListDto : EntityDto
    {
        private TripAccidentResolveListDto _resolveListDto;
        public int TripId { get; set; }
        public string Reason { get; set; }

        public DateTime CreationTime { get; set; }

        public TripAccidentResolveListDto ResolveListDto
        {
            get => _resolveListDto ?? new TripAccidentResolveListDto();
            set => _resolveListDto = value;
        }

        public bool IsResolve { get; set; }

        public bool IsPointStopped { get; set; }
    }
}