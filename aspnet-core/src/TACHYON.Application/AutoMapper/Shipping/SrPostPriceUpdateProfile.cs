using AutoMapper;
using TACHYON.Shipping.SrPostPriceUpdates;

namespace TACHYON.AutoMapper.Shipping
{
    public class SrPostPriceUpdateProfile : Profile
    {
        public SrPostPriceUpdateProfile()
        {
            CreateMap<SrPostPriceUpdate, SrPostPriceUpdateListDto>()
                .ForMember(x=> x.ActionTitle,x=>
                    x.MapFrom(i=> i.Action.GetEnumDescription()));

            CreateMap<SrPostPriceUpdate, ViewSrPostPriceUpdateDto>()
                .ForMember(x => x.ActionTitle, x =>
                    x.MapFrom(i => i.Action.GetEnumDescription()));

        }
    }
}