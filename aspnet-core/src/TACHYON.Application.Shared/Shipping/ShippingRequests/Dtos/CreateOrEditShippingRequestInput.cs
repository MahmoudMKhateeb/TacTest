using System;
using System.Collections.Generic;
using System.Text;
using TACHYON.Routs.Dtos;
using TACHYON.Routs.RoutPoints.Dtos;
using TACHYON.Vases.Dtos;

namespace TACHYON.Shipping.ShippingRequests.Dtos
{
    public class CreateOrEditShippingRequestInput
    {
        public List<CreateOrEditRoutPointInput> CreateOrEditRoutPointDtoList { get; set; }

        public virtual bool IsBid { get; set; }
        //Add Bid details If IsBid equals True
        public DateTime? BidStartDate { get; set; }
        public DateTime? BidEndDate { get; set; }

        public virtual bool IsTachyonDeal { get; set; }

        /// <summary>
        /// if we clone request
        /// </summary>
        public long? FatherShippingRequestId { get; set; }

        /// <summary>
        /// if assigned to carrier
        /// </summary>
        public int? CarrierTenantId { get; set; }

        public virtual int? TransportTypeId { get; set; }

        public virtual long TrucksTypeId { get; set; }

        public virtual int? CapacityId { get; set; }

        public int? GoodCategoryId { get; set; }

        public int? NumberOfDrops { get; set; }
        public bool StageOneFinish { get; set; }
        public bool StageTowFinish { get; set; }
        public bool StageThreeFinish { get; set; }

        public DateTime? StartTripDate { get; set; }
        public DateTime? EndTripDate { get; set; }

        public int NumberOfTrips { get; set; }
        public string PackingType { get; set; }
        public int NumberOfPacking { get; set; }
        public double TotalWeight { get; set; }
        public int ShippingTypeId { get; set; }


        //Route
        public CreateOrEditRouteDto CreateOrEditRouteDto { get; set; }


        //VasList
        public List<CreateOrEditShippingRequestVasListDto> ShippingRequestVasList { get; set; }



    }
}
