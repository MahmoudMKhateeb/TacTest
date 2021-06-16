using AutoMapper;
using TACHYON.Invoices.PaymentMethods;
using TACHYON.Invoices.PaymentMethods.Dto;

namespace TACHYON.AutoMapper.Invoices.PaymentMethods
{
   public class InvoicePaymentMethodProfile:Profile
    {
        public InvoicePaymentMethodProfile()
        {
            CreateMap<InvoicePaymentMethod, InvoicePaymentMethodListDto>();
            CreateMap<CreateOrEditInvoicePaymentMethod, InvoicePaymentMethod>();

        }
    }
}
