using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using TACHYON.Invoices;
using TACHYON.Invoices.Dto;
using TACHYON.Shipping.ShippingRequestTrips;

namespace TACHYON.AutoMapper.Invoices
{
    public class InvoiceProfile : Profile
    {
        public InvoiceProfile()
        {
            CreateMap<ShippingRequestTrip, InvoiceItemDto>()
                .ForMember(dst => dst.WayBillNumber, opt => opt.MapFrom(src => src.WaybillNumber))
                .ForMember(dst => dst.DateWork,
                    opt => opt.MapFrom(src =>
                        src.ShippingRequestFk.EndTripDate.HasValue
                            ? src.ShippingRequestFk.EndTripDate.Value.ToString("dd MMM, yyyy")
                            : ""))
                ;
            CreateMap<Invoice, InvoiceOutSideDto>()
               .ForMember(dst => dst.CompanyName, opt => opt.MapFrom(src => src.Tenant.Name));

        }
    }
}