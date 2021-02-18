using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using TACHYON.AddressBook.Dtos;
using TACHYON.Routs.RoutSteps;

namespace TACHYON.Routs.RoutPoints.Dtos
{
    public class RoutPointDto: EntityDto<long>
    {
        public string DisplayName { get; set; }

        public int? PickingTypeId { get; set; }

        //public string Location { get; set; }

        public long FacilityId { get; set; }

        public FacilityDto FacilityDto { get; set; }

        public long ShippingRequestId { get; set; }


        //to do receiver attribute
    }
}
