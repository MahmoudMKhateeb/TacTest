using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using TACHYON.DedicatedDynamicActorInvoices.DedicatedDynamicActorInvoiceItems;
using TACHYON.DedicatedDynamicActorInvoices;
using TACHYON.DedidcatedDynamicActorInvoices.Dtos;

namespace TACHYON.AutoMapper.DedicatedDynamicInvoices
{
    public class DedicatedDynamicActorInvoiceProfile : Profile
    {
        public DedicatedDynamicActorInvoiceProfile()
        {
            CreateMap<DedicatedDynamicActorInvoice, CreateOrEditDedicatedActorInvoiceDto>()
              .ForMember(x => x.DedicatedActorInvoiceItems, x => x.MapFrom(i => i.DedicatedDynamicActorInvoiceItems));

            CreateMap<DedicatedDynamicActorInvoice, DedicatedDynamicActorInvoiceDto>()
            .ForMember(x => x.ShipperActor, x => x.MapFrom(i => i.ShipperActorFk !=null ?i.ShipperActorFk.CompanyName :""))
            .ForMember(x => x.CarrierActor, x => x.MapFrom(i => i.CarrierActorFk != null ? i.CarrierActorFk.CompanyName : ""))
            .ForMember(x => x.InvoiceNumber, x => x.MapFrom(i => i.ActorInvoice != null ? i.ActorInvoice.InvoiceNumber :""))
             .ForMember(x => x.SubmitInvoiceNumber, x => x.MapFrom(i => i.ActorSubmitInvoice.ReferencNumber))
            .ForMember(x => x.InvoiceAccountName, x => x.MapFrom(i => i.InvoiceAccountType.GetEnumDescription()))
            .ForMember(x => x.ShippingRequestReference, x => x.MapFrom(i => i.ShippingRequest.ReferenceNumber));

            CreateMap<CreateOrEditDedicatedActorInvoiceItemDto, DedicatedDynamicActorInvoiceItem>().ReverseMap();
        }
    }
}
