using Abp.Application.Services;
using Abp.Application.Services.Dto;
using System.Threading.Tasks;
using TACHYON.Common.Dto;
using TACHYON.Invoices.PaymentMethods.Dto;

namespace TACHYON.Invoices.PaymentMethods
{
    public interface IInvoicePaymentMethodAppService: IApplicationService
    {
        ListResultDto<InvoicePaymentMethodListDto> GetAll(FilterInput Input);
        Task<CreateOrEditInvoicePaymentMethod> CreateOrEdit(CreateOrEditInvoicePaymentMethod input);
        Task Delete(EntityDto input);
    }
}
