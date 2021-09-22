using Abp.Application.Services.Dto;

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
    }
}