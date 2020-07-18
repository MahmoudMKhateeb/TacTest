
using System;
using Abp.Application.Services.Dto;

namespace TACHYON.Trucks.Dtos
{
    public class TruckDto : EntityDto<Guid>
    {
		public string PlateNumber { get; set; }

		public string ModelName { get; set; }

		public string ModelYear { get; set; }

		public string LicenseNumber { get; set; }

		public DateTime LicenseExpirationDate { get; set; }

		public bool IsAttachable { get; set; }

		public string Note { get; set; }


		 public Guid TrucksTypeId { get; set; }

		 		 public Guid TruckStatusId { get; set; }

		 		 public long? Driver1UserId { get; set; }

		 		 public long? Driver2UserId { get; set; }

		 
    }
}