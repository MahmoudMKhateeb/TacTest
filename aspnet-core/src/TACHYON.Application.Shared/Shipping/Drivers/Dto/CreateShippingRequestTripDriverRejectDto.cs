using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.Shipping.Drivers.Dto
{
    public  class CreateShippingRequestTripDriverRejectDto: EntityDto
    {
        public int? ReasoneId { get; set; }
        [StringLength(500)]
        public string Description { get; set; }
    }
}
