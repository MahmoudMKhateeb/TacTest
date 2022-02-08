using AutoMapper;
using TACHYON.Shipping.ShippingRequestUpdate;
using TACHYON.Shipping.ShippingRequestUpdates;

namespace TACHYON.AutoMapper.Shipping
{
    public class ShippingRequestUpdateProfile : Profile
    {
        public ShippingRequestUpdateProfile()
        {
            CreateMap<ShippingRequestUpdate, ShippingRequestUpdateListDto>();
        }
    }
}