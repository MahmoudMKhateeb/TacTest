using System.ComponentModel.DataAnnotations;

namespace TACHYON.PricePackages.Dto.TmsPricePackages
{
    public class GetTmsPricePackagesInput
    {
        [Required]
        public string LoadOptions { get; set; }

        [Required]
        public int DestinationTenantId { get; set; }

        public int? ProposalId { get; set; }
        public int? AppendixId { get; set; }
    }
}