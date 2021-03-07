using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Abp.Application.Services.Dto;
namespace TACHYON.Shipping.Drivers.Dto
{
    public class ShippingRequestTripDriverDetailsDto: EntityDto<long>
    {
        public ICollection<ShippingRequestTripDriverRoutePointDto> RoutePoints { get; set; }
        public DateTime StartTripDate { get; set; }
        public string PackingType { get; set; }

        public double TotalWeight { get; set; }

        public string Note { get; set; }


    }
}
