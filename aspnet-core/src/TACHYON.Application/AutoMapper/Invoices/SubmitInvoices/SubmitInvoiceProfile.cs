using AutoMapper;
using TACHYON.Common;
using TACHYON.Invoices.Groups;
using TACHYON.Invoices.SubmitInvoices;
using TACHYON.Invoices.SubmitInvoices.Dto;

namespace TACHYON.AutoMapper.Invoices.SubmitInvoices
{
    public class SubmitInvoiceProfile : Profile
    {
        public SubmitInvoiceProfile()
        {
            CreateMap<SubmitInvoiceClaimCreateInput, DocumentUpload>();
            CreateMap<IHasDocument, SubmitInvoice>().ReverseMap();
        }
    }
}