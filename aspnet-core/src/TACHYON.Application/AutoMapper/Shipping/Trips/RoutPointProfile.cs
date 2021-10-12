using AutoMapper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using TACHYON.Goods.GoodsDetails;
using TACHYON.Goods.GoodsDetails.Dtos;
using TACHYON.Mobile;
using TACHYON.Routs.RoutPoints;
using TACHYON.Routs.RoutPoints.Dtos;
using TACHYON.Shipping.Drivers.Dto;

namespace TACHYON.AutoMapper.Shipping.Trips
{
    public class RoutPointProfile : Profile
    {
        public RoutPointProfile()
        {
            CreateMap<RoutPoint, RoutDropOffDto>()
                .ForPath(dest => dest.Facility, opt => opt.MapFrom(src => src.FacilityFk.Name))
                .ForPath(dest => dest.Address,
                    opt => opt.MapFrom(src => $"{src.FacilityFk.CityFk.DisplayName}-{src.FacilityFk.Address}"))
                .ForPath(dest => dest.Latitude, opt => opt.MapFrom(src => src.FacilityFk.Location.Y))
                .ForPath(dest => dest.Longitude, opt => opt.MapFrom(src => src.FacilityFk.Location.X))
                //.ForPath(dest => dest.TotalWeight, opt => opt.MapFrom(src => src.GoodsDetails.Sum(x=>x.Weight)))
                .ForPath(dest => dest.GoodsDetailListDto, opt => opt.MapFrom(src => src.GoodsDetails))
                .ForPath(dest => dest.PackagingType,
                    opt => opt.MapFrom(src => src.ShippingRequestTripFk.ShippingRequestFk.PackingTypeFk.DisplayName))
                .ForPath(dest => dest.ReceiverDto, opt => opt.MapFrom(src => src.ReceiverFk))
                .ForPath(dest => dest.ReceiverFullName, opt => opt.MapFrom(src => src.ReceiverFullName))
                //.ForPath(dest => dest.ReceiverEmailAddress, opt => opt.MapFrom(src => src.ReceiverFk != null ? src.ReceiverFk.EmailAddress : src.ReceiverEmailAddress))
                .ForPath(dest => dest.ReceiverPhoneNumber, opt => opt.MapFrom(src => src.ReceiverPhoneNumber))
                .ForPath(dest => dest.ReceiverCardIdNumber, opt => opt.MapFrom(src => src.ReceiverCardIdNumber))
                .ForPath(dest => dest.Rating, opt => opt.MapFrom(src => src.FacilityFk.Rate))
                .ForPath(dest => dest.RatingNumber, opt => opt.MapFrom(src => src.FacilityFk.RateNumber));

            CreateMap<RoutPoint, DropOffPointDto>();
            CreateMap<UserOTP, UserOtpDto>();
            CreateMap<GoodsDetail, GoodsDetailDto>()
                .ForPath(dest => dest.UnitOfMeasure,
                    opt => opt.MapFrom(src =>
                        src.UnitOfMeasureFk != null ? src.UnitOfMeasureFk.DisplayName : string.Empty))
                .ForMember(dest => dest.GoodCategory,
                    opt =>
                        opt.MapFrom(src => src.GoodCategoryFk != null
                            ? src.GoodCategoryFk.Translations.FirstOrDefault(x =>
                                x.Language.Contains(CultureInfo.CurrentCulture.Name)).DisplayName
                            : string.Empty))
                .ForMember(dest => dest.DangerousGoodTypeName,
                    opt => opt.MapFrom(src =>
                        src.DangerousGoodTypeFk != null ? src.DangerousGoodTypeFk.Name : string.Empty));
            //.ForPath(dest => dest.GoodCategory, opt => opt.MapFrom(src => src.GoodCategoryFk != null ? src.GoodCategoryFk.DisplayName : string.Empty));

        }
    }
}