using Abp.Application.Services.Dto;
using System.Collections.Generic;
using TACHYON.ServiceAreas;

namespace TACHYON.Authorization.Users.Profile.Dto
{
    public class TenantProfileInformationForViewDto : EntityDto
    {
        public string CompanyName { get; set; }

        public string CompanyInfo { get; set; }

        public string CompanyEmailAddress { get; set; }

        public string CompanySite { get; set; }

        public string CompanyPhone { get; set; }

        public string TenancyName { get; set; }

        public string CityName { get; set; }

        public string CountryName { get; set; }

        public double Rating { get; set; }

        public int RateNumber { get; set; }
        public decimal FacilitiesRating { get; set; }

        public int EditionId { get; set; }

        public List<ServiceAreaListItemDto> ServiceAreas { get; set; }
        
    }
}