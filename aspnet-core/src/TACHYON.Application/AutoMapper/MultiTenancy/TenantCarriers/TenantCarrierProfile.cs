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
                    .ForMember(dst => dst.CarrierName, opt => opt.MapFrom(src => src.CarrierShipper.Name));
            CreateMap<CreateTenantCarrierInput, TenantCarrier>();
            
        }
    }
}
