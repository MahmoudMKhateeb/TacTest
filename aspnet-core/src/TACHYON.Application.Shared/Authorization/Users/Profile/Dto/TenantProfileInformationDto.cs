using Abp.Application.Services.Dto;

namespace TACHYON.Authorization.Users.Profile.Dto
{
    public class TenantProfileInformationDto : EntityDto
    {
        public string CompanyName { get; set; }

        public string CompanyInfo { get; set; }

        public string CompanyEmailAddress { get; set; }

        public string CompanySite { get; set; }

        public string CompanyPhone { get; set; }

        public int EditionId { get; set; }

        public int CountryId { get; set; }

        public int CityId { get; set; }

        public decimal Rating { get; set; }
    }
}