using Abp.Application.Editions;
using Abp.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace TACHYON.Localization
{
    [Table("TerminologieEditions")]
    public class TerminologieEdition : Entity
    {
        public int EditionId { get; set; }
        [ForeignKey(nameof(EditionId))] public Edition Edition { get; set; }

        public int TerminologieId { get; set; }
        [ForeignKey(nameof(TerminologieId))] public AppLocalization Terminologie { get; set; }
    }
}