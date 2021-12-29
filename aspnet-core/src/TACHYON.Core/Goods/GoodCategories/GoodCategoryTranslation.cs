using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using TACHYON.Common;

namespace TACHYON.Goods.GoodCategories
{
    [Table("GoodsCategoryTranslations")]
    public class GoodCategoryTranslation : Entity, IEntityTranslation<GoodCategory>, IHasDisplayName
    {
        public string DisplayName { get; set; }
        public GoodCategory Core { get; set; }
        public int CoreId { get; set; }
        public string Language { get; set; }
    }
}