using Abp.Application.Services.Dto;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TACHYON.Shipping.ShippingRequests.Dtos
{
    public class ShippingRequestDestinationCitiesDto : EntityDto<int?>
    {
        public int CityId { get; set; }
        /// <summary>
        /// Front helper
        /// </summary>
        public string CityName { get; set; }
    }
}
