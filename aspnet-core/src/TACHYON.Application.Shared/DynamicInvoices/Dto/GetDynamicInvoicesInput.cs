using TACHYON.Dto;

namespace TACHYON.DynamicInvoices.Dto
{
    public class GetDynamicInvoicesInput : PagedSortedAndFilteredInputDto
    {
        public long? WaybillNumber { get; set; }
        
    }
}