using System.ComponentModel.DataAnnotations;

namespace TACHYON.PricePackages.Dto
{
    public class GetPricePackagesInput
    {
        [Required]
        public string LoadOptions { get; set; }

        [Required]
        public int DestinationTenantId { get; set; }

        public int? ProposalId { get; set; }
    }
}