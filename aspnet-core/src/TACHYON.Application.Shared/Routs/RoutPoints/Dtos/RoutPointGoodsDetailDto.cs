using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TACHYON.Routs.RoutPoints.Dtos
{
    public class RoutPointGoodsDetailDto
    {
        [Required]
        public int Amount{get; set;}
        [Required]
        public long GoodsDetailsId { get; set; }
        [Required]
        public long RoutPointId { get; set; }

    }
}
