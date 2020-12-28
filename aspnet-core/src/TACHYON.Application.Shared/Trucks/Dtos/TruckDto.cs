
using Abp.Application.Services.Dto;
using System;

namespace TACHYON.Trucks.Dtos
{
    public class TruckDto : EntityDto<Guid>
    {
        public string PlateNumber { get; set; }

        public string ModelName { get; set; }

        public string ModelYear { get; set; }

        public string Note { get; set; }

        public virtual string Capacity { get; set; }
        public long TrucksTypeId { get; set; }

        public long TruckStatusId { get; set; }
        public int? Length { get; set; }

    }
}