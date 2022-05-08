using Abp.Application.Services.Dto;
using Abp.Runtime.Validation;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.Authorization.Users.Profile.Dto
{
    public class UpdateTenantProfileInformationInputDto : EntityDto
    {
        [Required]
        [StringLength(80, MinimumLength = 2)]
        public string CompanyName { get; set; }

        [StringLength(500)]
        public string CompanyInfo { get; set; }

        [Required]
        [EmailAddress]
        public string CompanyEmailAddress { get; set; }

        public string CompanySite { get; set; }
        
        [Required]
        [StringLength(20)]
        public string CompanyPhone { get; set; }
    }
}