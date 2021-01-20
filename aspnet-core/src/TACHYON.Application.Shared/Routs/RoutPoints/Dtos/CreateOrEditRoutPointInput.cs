using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TACHYON.Routs.RoutPoints.Dtos
{
    public class CreateOrEditRoutPointInput
    {
        public long? Id { get; set; }
        // public string Longitude{get; set;}
        //public virtual string  Longitude { get; set; }
        [Required]
        [RegularExpression(@"^(-?\d + (\.\d +)?),\s*(-?\d+(\.\d+)?)$")]
        public string Location { get; set; }
        public string DisplayName { get; set; }
        [Required]
        public int CityId { get; set; }
        public int? PickingTypeId { get; set; }
        [Required]
        public long FacilityId { get; set; }

        public List<RoutPointGoodsDetailDto> RoutPointGoodsDetailListDto { get; set; }
    }
}
