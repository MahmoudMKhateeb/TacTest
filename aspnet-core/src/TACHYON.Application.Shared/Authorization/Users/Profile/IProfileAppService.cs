using Abp.Application.Services;
using Abp.Application.Services.Dto;
using System.Threading.Tasks;
using TACHYON.AddressBook.Dtos;
using TACHYON.Authorization.Users.Dto;
using TACHYON.Authorization.Users.Profile.Dto;
using TACHYON.Vases.Dtos;

namespace TACHYON.Authorization.Users.Profile
{
    public interface IProfileAppService : IApplicationService
    {
        Task<CurrentUserProfileEditDto> GetCurrentUserProfileForEdit();

        Task UpdateCurrentUserProfile(CurrentUserProfileEditDto input);

        Task ChangePassword(ChangePasswordInput input);

        Task UpdateProfilePicture(UpdateProfilePictureInput input);

        Task<GetPasswordComplexitySettingOutput> GetPasswordComplexitySetting();

        Task<GetProfilePictureOutput> GetProfilePicture(long? userId);

        Task<GetProfilePictureOutput> GetProfilePictureByUser(long userId);

        Task<GetProfilePictureOutput> GetProfilePictureByUserName(string username);

        Task<GetProfilePictureOutput> GetFriendProfilePicture(GetFriendProfilePictureInput input);

        Task ChangeLanguage(ChangeUserLanguageDto input);

        Task<UpdateGoogleAuthenticatorKeyOutput> UpdateGoogleAuthenticatorKey();

        Task SendVerificationSms(SendVerificationSmsInputDto input);

        Task VerifySmsCode(VerifySmsCodeInputDto input);

        Task PrepareCollectedData();

        Task<GetTenantProfileInformationForViewDto> GetTenantProfileInformationForView(int tenantId);

        Task<GetTenantProfileInformationForEditDto> GetTenantProfileInformationForEdit(int tenantId);

        Task UpdateTenantProfileInformation(UpdateTenantProfileInformationInputDto input);

        Task<int> GetShipmentCount();

        Task<PagedResultDto<FacilityLocationListDto>> GetFacilitiesInformation(GetFacilitiesInformationInput input);

        Task<InvoicingInformationDto> GetInvoicingInformation();

        Task<FleetInformationDto> GetFleetInformation(GetFleetInformationInputDto input);

        Task<PagedResultDto<AvailableVasDto>> GetAvailableVases(GetAvailableVasesInputDto input);
    }
}