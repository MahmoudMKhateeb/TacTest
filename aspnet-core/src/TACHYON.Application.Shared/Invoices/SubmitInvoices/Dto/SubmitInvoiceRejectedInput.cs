using Abp.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.Invoices.Groups.Dto
{
    public class SubmitInvoiceRejectedInput : Entity<long>
    {
        [Required]
        [StringLength(500,MinimumLength =10)]
        public string Reason { get; set; }
    }

}
