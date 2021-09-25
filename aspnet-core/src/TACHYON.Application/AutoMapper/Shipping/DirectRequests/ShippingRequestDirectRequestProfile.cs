using AutoMapper;
using TACHYON.Shipping.DirectRequests;
using TACHYON.Shipping.DirectRequests.Dto;

namespace TACHYON.AutoMapper.Shipping.DirectRequests
{
    public class ShippingRequestDirectRequestProfile : Profile
    {
        public ShippingRequestDirectRequestProfile()
        {

            CreateMap<ShippingRequestDirectRequest, ShippingRequestDirectRequestListDto>()
            .ForMember(dest => dest.Carrier, opt => opt.MapFrom(src => src.Carrier.Name))
            .ForMember(dest => dest.TenantId, opt => opt.MapFrom(src => src.Carrier.Id));
            CreateMap<CreateShippingRequestDirectRequestInput, ShippingRequestDirectRequest>();

        }
    }
}