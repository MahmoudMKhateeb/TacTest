
using Abp.Application.Services.Dto;
using System;

namespace TACHYON.Trucks.Dtos
{
    public class TruckDto : EntityDto<Guid>
    {
        public string PlateNumber { get; set; }

        public string ModelName { get; set; }

        public string ModelYear { get; set; }

        //public string LicenseNumber { get; set; }

        //public DateTime LicenseExpirationDate { get; set; }

        //public bool IsAttachable { get; set; }

        public string Note { get; set; }

        public virtual string Capacity { get; set; }
        public long TrucksTypeId { get; set; }

        public long TruckStatusId { get; set; }

        public long? Driver1UserId { get; set; }

        //public long? Driver2UserId { get; set; }

        //public int? RentPrice { get; set; }

        //public int? RentDuration { get; set; }

    }
}