using Abp.Localization;
using AutoMapper;
using TACHYON.Extension;
using TACHYON.Shipping.ShippingRequestUpdate;
using TACHYON.Shipping.ShippingRequestUpdates;

namespace TACHYON.AutoMapper.Shipping
{
    public class ShippingRequestUpdateProfile : Profile
    {
        public ShippingRequestUpdateProfile()
        {
            CreateMap<ShippingRequestUpdate, ShippingRequestUpdateListDto>()
                .ForMember(x=> x.StatusTitle,x=> 
                    x.MapFrom(i=> new LocalizableString(i.Status.GetEnumDescription(),TACHYONConsts.LocalizationSourceName)));
        }
    }
}