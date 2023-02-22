using DevExtreme.AspNet.Data.ResponseModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TACHYON.DedidcatedDynamicActorInvoices.Dtos;
using TACHYON.Invoices;

namespace TACHYON.DedidcatedDynamicActorInvoices
{
    public interface IDedicatedDynamiceActorInvoicesAppService
    {
        Task<LoadResult> GetAll(string filter);
        Task CreateOrEdit(CreateOrEditDedicatedActorInvoiceDto input);
        Task<CreateOrEditDedicatedActorInvoiceDto> GetDedicatedInvoiceForEdit(long id);
        Task Delete(long id);
        Task GenerateDedicatedInvoice(long id);
        Task<decimal> GetDedicatePricePerDay(long ShippingRequestId, InvoiceAccountType invoiceAccountType, int AllNumberOfDays);


    }
}
