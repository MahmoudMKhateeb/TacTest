using Abp.Application.Services.Dto;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TACHYON.ServiceAreas;

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
        public string FinancialName { get; set; }
        public string FinancialPhone { get; set; }
        public string FinancialEmail { get; set; }
        public List<CreateServiceAreaDto> CityServiceAreas { get; set; }
        
    }
}