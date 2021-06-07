using AutoMapper;
using TACHYON.MultiTenancy;
using TACHYON.MultiTenancy.TenantCarriers.Dto;

namespace TACHYON.AutoMapper.MultiTenancy.TenantCarriers
{
    public class TenantCarrierProfile:Profile
    {
        public TenantCarrierProfile()
        {
            CreateMap<TenantCarrier, TenantCarriersListDto>()
                    .ForMember(dst => dst.CarrierName, opt => opt.MapFrom(src => src.CarrierShipper.companyName));
            CreateMap<CreateTenantCarrierInput, TenantCarrier>()
                    .ForMember(dst => dst.TenantId, opt => opt.MapFrom(src => src.Id));
            
        }
    }
}
