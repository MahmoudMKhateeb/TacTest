using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.Shipping.ShippingRequests.Dtos
{
    public class CancelShippingRequestInput : EntityDto<long>
    {
        [Required]
        [StringLength(500)]
        public string CancelReason { get; set; }
    }
}
