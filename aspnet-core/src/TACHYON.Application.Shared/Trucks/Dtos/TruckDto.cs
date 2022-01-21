using Abp.Application.Services.Dto;


namespace TACHYON.Trucks.Dtos
{
    public class TruckDto : EntityDto<long>
    {
        public string PlateNumber { get; set; }

        public string ModelName { get; set; }

        public string ModelYear { get; set; }

        public string Note { get; set; }

        public virtual string Capacity { get; set; }

        public int? Length { get; set; }

        public string TruckStatusDisplayName { get; set; }

        #region Truck Categories

        public virtual int? TransportTypeId { get; set; }
        public string TransportTypeDisplayName { get; set; }
        public virtual long? TrucksTypeId { get; set; }
        public string TrucksTypeDisplayName { get; set; }
        public virtual int? CapacityId { get; set; }
        public string CapacityDisplayName { get; set; }

        #endregion

        public bool IsMissingDocumentFiles { get; set; }

        public string CompanyName { get; set; }

        //document file Number
        public string IstmaraNumber { get; set; }
    }
}