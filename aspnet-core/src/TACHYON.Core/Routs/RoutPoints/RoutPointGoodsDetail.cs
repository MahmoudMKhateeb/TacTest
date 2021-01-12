using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using TACHYON.Goods.GoodsDetails;

namespace TACHYON.Routs.RoutPoints
{
    [Table("RoutPointGoodsDetails")]
    public class RoutPointGoodsDetail: FullAuditedEntity<long>
    {
        [Required]
        public long GoodsDetailsId { get; set; }

        [ForeignKey("GoodsDetailsId")]
        public GoodsDetail GoodsDetailsFk { get; set; }

        [Required]
        public long RoutPointId { get; set; }

        [ForeignKey("RoutPointId")]
        public RoutPoint RoutPointFk { get; set; }

        /// <summary>
        /// Amount whick is picked or dropped off
        /// </summary>
        [Required]
        public double Amount { get; set; }
    }
}
