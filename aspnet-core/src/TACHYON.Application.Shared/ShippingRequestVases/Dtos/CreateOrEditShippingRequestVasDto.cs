using Abp.Application.Services.Dto;

namespace TACHYON.ShippingRequestVases.Dtos
{
    public class CreateOrEditShippingRequestVasDto : EntityDto<long?>
    {
        public int VasId { get; set; }
        public virtual int RequestMaxAmount { get; set; }
        public virtual int RequestMaxCount { get; set; }
        public int NumberOfTrips { get; set; }
        public string OtherVasName { get; set; }


    }
}