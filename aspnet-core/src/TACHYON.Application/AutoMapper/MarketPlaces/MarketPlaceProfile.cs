using Abp.Timing;
using AutoMapper;
using System;
using TACHYON.MarketPlaces.Dto;
using TACHYON.Shipping.ShippingRequests;

namespace TACHYON.AutoMapper.MarketPlaces
{
    public class MarketPlaceProfile: Profile
    {
        public MarketPlaceProfile()
        {
            CreateMap<ShippingRequest, MarketPlaceListDto>()
                 .ForMember(dst => dst.Shipper, opt => opt.MapFrom(src => src.Tenant.Name))
                 .ForMember(dst => dst.OriginCity, opt => opt.MapFrom(src => src.OriginCityFk.DisplayName ))
                 .ForMember(dst => dst.DestinationCity, opt => opt.MapFrom(src => src.DestinationCityFk.DisplayName))
                 .ForMember(dst => dst.RemainingDays, opt => opt.MapFrom(src => GetRemainingDays(src.BidEndDate)))
                 .ForMember(dst => dst.RangeDate, opt => opt.MapFrom(src => GetDateRange(src.StartTripDate,src.EndTripDate)))
                 ;
        }
        private string GetRemainingDays(DateTime? BidEndDate)
        {
            if (BidEndDate.HasValue)
            {
                int TotalDays = (int)((BidEndDate.Value - Clock.Now).TotalDays);
                if (TotalDays <= 0) return "0";
                if (TotalDays<9) return $"0{TotalDays}";
                return TotalDays.ToString();
            }
            return "";
        }

        private string GetDateRange(DateTime? StartTripDate, DateTime? EndTripDate)
        {
            if (StartTripDate.HasValue && EndTripDate.HasValue)
            {
                return string.Format("{0}-{1}", StartTripDate.Value.ToString("dd/MM/yyyy"), EndTripDate.Value.ToString("dd/MM/yyyy"));
            }
            else if (StartTripDate.HasValue) return StartTripDate.Value.ToString("dd/MM/yyyy");
            return "";
        }
    }
}
