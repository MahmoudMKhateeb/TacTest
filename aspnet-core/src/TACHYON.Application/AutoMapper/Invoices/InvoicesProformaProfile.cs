using AutoMapper;
using TACHYON.Invoices;
using TACHYON.Invoices.InvoicesProformas.dto;

namespace TACHYON.AutoMapper.Invoices
{
    public class InvoicesProformaProfile : Profile
    {
        public InvoicesProformaProfile()
        {
            CreateMap<InvoiceProforma, InvoicesProformaListDto>()
                .ForMember(dto => dto.ClientName, options => options.MapFrom(entity => entity.Tenant.Name));
        }
    }
}