using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.DynamicInvoices.DynamicInvoiceItems
{
    public class CreateOrEditDynamicInvoiceItemDto : EntityDto<long?>
    {
        [Required]
        [StringLength(256,MinimumLength = 2)]
        public string Description { get; set; }
        
        [Required]
        public decimal Price { get; set; }
        
    }
}