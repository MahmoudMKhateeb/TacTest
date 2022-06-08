using Abp.Application.Services.Dto;

namespace TACHYON.Shipping.Trips.Accidents.Dto
{
    public class TripAccidentResolveListDto : EntityDto<int?>
    {
        public bool IsAppliedResolve { get; set; }

        public TripAccidentResolveType? ResolveType { get; set; }
        
        public string ResolveTypeTitle { get; set; }
        
        public bool ApprovedByShipper { get; set; }
        
        public bool ApprovedByCarrier { get; set; }
        
        public string ResolveStatus
        {
            get
            {
                if (Id.HasValue && IsAppliedResolve)
                    return "ResolveApplied";
                if (Id.HasValue && !IsAppliedResolve && !ApprovedByCarrier && !ApprovedByShipper)
                    return "ResolveNotApplied";
                if (Id.HasValue && !IsAppliedResolve && ApprovedByCarrier && !ApprovedByShipper )
                    return "WaitAcceptanceFromShipper";
                if (Id.HasValue && !IsAppliedResolve && !ApprovedByCarrier && ApprovedByShipper )
                    return "WaitAcceptanceFromCarrier";
                
                return "NoResolveYet";
            }
        }
    }
}