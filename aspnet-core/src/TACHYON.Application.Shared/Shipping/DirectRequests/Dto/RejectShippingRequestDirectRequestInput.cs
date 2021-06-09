using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.Shipping.DirectRequests.Dto
{
    public class RejectShippingRequestDirectRequestInput : EntityDto<long>
    {
        [Required]
        public string Reason { get; set; }
    }
}
