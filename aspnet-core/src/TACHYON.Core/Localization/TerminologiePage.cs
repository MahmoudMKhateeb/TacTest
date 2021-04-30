using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TACHYON.Localization
{
    [Table("TerminologiePages")]
    public class TerminologiePage : Entity
    {
        public string PageUrl { get; set; }
        public int TerminologieId { get; set; }
        [ForeignKey(nameof(TerminologieId))]
        public AppLocalization Terminologie { get; set; }
    }
}
