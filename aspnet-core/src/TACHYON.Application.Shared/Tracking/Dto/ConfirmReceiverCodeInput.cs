using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.Tracking.Dto
{
    public class ConfirmReceiverCodeInput : EntityDto<long>
    {
        [Required]
        public string Code { get; set; }
        public string Action { get; set; }

    }
}