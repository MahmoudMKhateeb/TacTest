using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using TACHYON.DedicatedDynamicInvoices.DedicatedDynamicInvoiceItems;
using TACHYON.DedicatedDynamicInvoices.Dtos;
using TACHYON.DedicatedInvoices;

namespace TACHYON.AutoMapper.DedicatedDynamicInvoices
{
    public class DedicatedDynamicInvoiceProfile : Profile
    {
        public DedicatedDynamicInvoiceProfile()
        {
            CreateMap<DedicatedDynamicInvoice, CreateOrEditDedicatedInvoiceDto>()
              .ForMember(x => x.DedicatedInvoiceItems, x => x.MapFrom(i => i.DedicatedDynamicInvoiceItems));

            CreateMap<DedicatedDynamicInvoice, DedicatedDynamicInvoiceDto>()
            .ForMember(x => x.TenantName, x => x.MapFrom(i => i.Tenant.Name))
            .ForMember(x => x.InvoiceNumber, x => x.MapFrom(i => i.Invoice.InvoiceNumber))
             .ForMember(x => x.SubmitInvoiceNumber, x => x.MapFrom(i => i.SubmitInvoice.ReferencNumber))
            .ForMember(x => x.InvoiceAccountName, x => x.MapFrom(i => i.InvoiceAccountType.GetEnumDescription()))
            .ForMember(x => x.ShippingRequestReference, x => x.MapFrom(i => i.ShippingRequest.ReferenceNumber));

            CreateMap<CreateOrEditDedicatedInvoiceItemDto, DedicatedDynamicInvoiceItem>().ReverseMap();
        }
    }
}
