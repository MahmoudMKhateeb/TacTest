using System;
using System.Collections.Generic;
using System.Text;
using TACHYON.AddressBook.Dtos;

namespace TACHYON.Routs.RoutPoints.Dtos
{
    public class GetRoutPointForViewOutput
    {
        public RoutPointDto RoutPointDto { get; set; }
        public string PickingTypeDisplayName{get; set;}
        public GetFacilityForViewOutput facilityDto { get; set; }
        public List<RoutPointGoodsDetailDto> RoutPointGoodsDetailsList { get; set; }
    }
}
