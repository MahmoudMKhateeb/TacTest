using Abp;
using Abp.Application.Editions;
using Abp.Application.Features;
using Abp.Application.Services.Dto;
using Abp.Auditing;
using Abp.Authorization;
using Abp.Authorization.Users;
using Abp.BackgroundJobs;
using Abp.Configuration;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Abp.Localization;
using Abp.Runtime.Caching;
using Abp.Runtime.Session;
using Abp.Timing;
using Abp.UI;
using Abp.Zero.Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using TACHYON.AddressBook;
using TACHYON.AddressBook.Dtos;
using TACHYON.Authentication.TwoFactor.Google;
using TACHYON.Authorization.Users.Dto;
using TACHYON.Authorization.Users.Profile.Cache;
using TACHYON.Authorization.Users.Profile.Dto;
using TACHYON.Cities;
using TACHYON.Configuration;
using TACHYON.Features;
using TACHYON.Friendships;
using TACHYON.Gdpr;
using TACHYON.Invoices.PaymentMethods;
using TACHYON.Net.Sms;
using TACHYON.Security;
using TACHYON.Shipping.ShippingRequestTrips;
using TACHYON.Shipping.Trips;
using TACHYON.Storage;
using TACHYON.Timing;
using TACHYON.Trucks;
using TACHYON.Trucks.TrucksTypes;
using TACHYON.Trucks.TrucksTypes.Dtos;
using TACHYON.Trucks.TrucksTypes.TrucksTypesTranslations;
using TACHYON.Vases;
using TACHYON.Vases.Dtos;

namespace TACHYON.Authorization.Users.Profile
{
    [AbpAuthorize]
    public class ProfileAppService : TACHYONAppServiceBase, IProfileAppService
    {
        private const int MaxProfilPictureBytes = 5242880; //5MB
        private readonly IBinaryObjectManager _binaryObjectManager;
        private readonly ITimeZoneService _timeZoneService;
        private readonly IFriendshipManager _friendshipManager;
        private readonly GoogleTwoFactorAuthenticateService _googleTwoFactorAuthenticateService;
        private readonly ISmsSender _smsSender;
        private readonly ICacheManager _cacheManager;
        private readonly ITempFileCacheManager _tempFileCacheManager;
        private readonly IBackgroundJobManager _backgroundJobManager;
        private readonly ProfileImageServiceFactory _profileImageServiceFactory;
        private readonly IRepository<User, long> _lookupUserRepository;
        private readonly IRepository<ShippingRequestTrip> _lookupTripRepository;
        private readonly IRepository<Facility, long> _lookupFacilityRepository;
        private readonly IRepository<InvoicePaymentMethod> _lookupPaymentMethodRepository;
        private readonly IRepository<TrucksType, long> _lookupTruckTypeRepository;
        private readonly IRepository<Truck, long> _lookupTruckRepository;
        private readonly IRepository<VasPrice> _lookupVasPriceRepository;
        private readonly IRepository<Edition> _lookupEditionRepository;
        private readonly IRepository<City> _lookupCityRepository;
        private readonly IRepository<TrucksTypesTranslation> _trucksTypesTranslationRepository;

        public ProfileAppService(
            IAppFolders appFolders,
            IBinaryObjectManager binaryObjectManager,
            ITimeZoneService timezoneService,
            IFriendshipManager friendshipManager,
            GoogleTwoFactorAuthenticateService googleTwoFactorAuthenticateService,
            ISmsSender smsSender,
            ICacheManager cacheManager,
            ITempFileCacheManager tempFileCacheManager,
            IBackgroundJobManager backgroundJobManager,
            ProfileImageServiceFactory profileImageServiceFactory,
            IRepository<User, long> lookupUserRepository,
            IRepository<ShippingRequestTrip> lookupTripRepository,
            IRepository<Facility, long> lookupFacilityRepository,
            IRepository<InvoicePaymentMethod> lookupPaymentMethodRepository,
            IRepository<TrucksType, long> lookupTruckTypeRepository,
            IRepository<VasPrice> lookupVasPriceRepository,
            IRepository<Truck, long> lookupTruckRepository,
            IRepository<Edition> lookupEditionRepository,
            IRepository<City> lookupCityRepository, IRepository<TrucksTypesTranslation> trucksTypesTranslationRepository)
        {
            _binaryObjectManager = binaryObjectManager;
            _timeZoneService = timezoneService;
            _friendshipManager = friendshipManager;
            _googleTwoFactorAuthenticateService = googleTwoFactorAuthenticateService;
            _smsSender = smsSender;
            _cacheManager = cacheManager;
            _tempFileCacheManager = tempFileCacheManager;
            _backgroundJobManager = backgroundJobManager;
            _profileImageServiceFactory = profileImageServiceFactory;
            _lookupUserRepository = lookupUserRepository;
            _lookupTripRepository = lookupTripRepository;
            _lookupFacilityRepository = lookupFacilityRepository;
            _lookupPaymentMethodRepository = lookupPaymentMethodRepository;
            _lookupTruckTypeRepository = lookupTruckTypeRepository;
            _lookupVasPriceRepository = lookupVasPriceRepository;
            _lookupTruckRepository = lookupTruckRepository;
            _lookupEditionRepository = lookupEditionRepository;
            _lookupCityRepository = lookupCityRepository;
            _trucksTypesTranslationRepository = trucksTypesTranslationRepository;
        }

        [DisableAuditing]
        public async Task<CurrentUserProfileEditDto> GetCurrentUserProfileForEdit()
        {
            var user = await GetCurrentUserAsync();
            var userProfileEditDto = ObjectMapper.Map<CurrentUserProfileEditDto>(user);

            userProfileEditDto.QrCodeSetupImageUrl = user.GoogleAuthenticatorKey != null
                ? _googleTwoFactorAuthenticateService.GenerateSetupCode("TACHYON",
                    user.EmailAddress, user.GoogleAuthenticatorKey, 300, 300).QrCodeSetupImageUrl
                : "";
            userProfileEditDto.IsGoogleAuthenticatorEnabled = user.GoogleAuthenticatorKey != null;

            if (Clock.SupportsMultipleTimezone)
            {
                userProfileEditDto.Timezone = await SettingManager.GetSettingValueAsync(TimingSettingNames.TimeZone);

                var defaultTimeZoneId =
                    await _timeZoneService.GetDefaultTimezoneAsync(SettingScopes.User, AbpSession.TenantId);
                if (userProfileEditDto.Timezone == defaultTimeZoneId)
                {
                    userProfileEditDto.Timezone = string.Empty;
                }
            }

            return userProfileEditDto;
        }

        public async Task DisableGoogleAuthenticator()
        {
            var user = await GetCurrentUserAsync();
            user.GoogleAuthenticatorKey = null;
        }

        public async Task<UpdateGoogleAuthenticatorKeyOutput> UpdateGoogleAuthenticatorKey()
        {
            var user = await GetCurrentUserAsync();
            user.GoogleAuthenticatorKey = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 10);
            CheckErrors(await UserManager.UpdateAsync(user));

            return new UpdateGoogleAuthenticatorKeyOutput
            {
                QrCodeSetupImageUrl = _googleTwoFactorAuthenticateService.GenerateSetupCode(
                    "TACHYON",
                    user.EmailAddress, user.GoogleAuthenticatorKey, 300, 300).QrCodeSetupImageUrl
            };
        }

        public async Task SendVerificationSms(SendVerificationSmsInputDto input)
        {
            var code = RandomHelper.GetRandom(100000, 999999).ToString();
            var cacheKey = AbpSession.ToUserIdentifier().ToString();
            var cacheItem = new SmsVerificationCodeCacheItem { Code = code };

            _cacheManager.GetSmsVerificationCodeCache().Set(
                cacheKey,
                cacheItem
            );

            await _smsSender.SendAsync(input.PhoneNumber, L("SmsVerificationMessage", code));
        }

        public async Task VerifySmsCode(VerifySmsCodeInputDto input)
        {
            var cacheKey = AbpSession.ToUserIdentifier().ToString();
            var cash = await _cacheManager.GetSmsVerificationCodeCache().GetOrDefaultAsync(cacheKey);

            if (cash == null)
            {
                throw new Exception("Phone number confirmation code is not found in cache !");
            }

            if (input.Code != cash.Code)
            {
                throw new UserFriendlyException(L("WrongSmsVerificationCode"));
            }

            var user = await UserManager.GetUserAsync(AbpSession.ToUserIdentifier());
            user.IsPhoneNumberConfirmed = true;
            user.PhoneNumber = input.PhoneNumber;
            await UserManager.UpdateAsync(user);
        }

        public async Task PrepareCollectedData()
        {
            await _backgroundJobManager.EnqueueAsync<UserCollectedDataPrepareJob, UserIdentifier>(
                AbpSession.ToUserIdentifier());
        }

        #region SharedServices_Shipper_And_Carrier
        public async Task<TenantProfileInformationForViewDto> GetTenantProfileInformationForView(int tenantId)
        {
            var profile = await GetTenantProfileInformation(tenantId);
            var profileForView = ObjectMapper.Map<TenantProfileInformationForViewDto>(profile);
            profileForView.TenancyName = await _lookupEditionRepository.GetAll()
                .Where(x => x.Id == profile.EditionId)
                .Select(x => x.DisplayName).FirstOrDefaultAsync();
            profileForView.CityName = await GetCityNameAsync(profile.CityId);
            profileForView.CountryName = await GetCountryNameAsync(profile.CountryId);
            return profileForView;
        }

        public async Task<UpdateTenantProfileInformationInputDto> GetTenantProfileInformationForEdit()
        {
            if (!AbpSession.TenantId.HasValue)
                throw new UserFriendlyException(L("YouDontHaveAccessToThisPage"));
            return ObjectMapper.Map<UpdateTenantProfileInformationInputDto>(await GetTenantProfileInformation(AbpSession.TenantId.Value));
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_ProfileManagement)]
        public async Task UpdateTenantProfileInformation(UpdateTenantProfileInformationInputDto input)
        {
            if (!AbpSession.TenantId.HasValue || AbpSession.TenantId != input.Id)
                throw new UserFriendlyException(L("YouDontHaveAccessToThisPage"));

            var tenant = await TenantManager.GetByIdAsync(input.Id);
            var updatedTenant = ObjectMapper.Map(input, tenant);
            await TenantManager.UpdateAsync(updatedTenant);
            var oldEmail = await GetCompanyEmailAddress(input.Id);
            if (!input.CompanyEmailAddress.Equals(oldEmail))
            {
                var user = await UserManager.GetUserByEmailAsync(oldEmail);
                user.EmailAddress = input.CompanyEmailAddress;
                user.IsEmailConfirmed = false;

            }
        }

        public async Task<int> GetShipmentCount(int tenantId)
        {  // Two In One Service

            var tenant = await TenantManager.GetByIdAsync(tenantId);
            var editionName = await (from edition in _lookupEditionRepository.GetAll()
                                     where tenant.EditionId == edition.Id
                                     select edition.DisplayName).FirstOrDefaultAsync();

            var isShipper = editionName.ToUpper().Contains("SHIPPER");
            var isCarrier = editionName.ToUpper().Contains("CARRIER");

            if (!isShipper && !isCarrier)
                throw new UserFriendlyException(L("ThisTenantNotShipperOrCarrier"));

            var numberOfCompletedShipments = await _lookupTripRepository.GetAll()
                .Where(x => x.Status == ShippingRequestTripStatus.Intransit
                            || x.Status == ShippingRequestTripStatus.Delivered
                            || x.Status == ShippingRequestTripStatus.DeliveredAndNeedsConfirmation)
                .WhereIf(isShipper, x => x.ShippingRequestFk.TenantId == tenantId)
                .WhereIf(isCarrier, x => x.ShippingRequestFk.CarrierTenantId == tenantId)
                .CountAsync();

            return numberOfCompletedShipments;
        }

        #endregion

        #region ShipperServicesOnly


        public async Task<PagedResultDto<FacilityLocationListDto>> GetFacilitiesInformation(GetFacilitiesInformationInput input)
        {
            var shipperFacilities = _lookupFacilityRepository.GetAll()
                .AsNoTracking().Include(x => x.CityFk)
                .Where(x => x.TenantId == input.TenantId)
                .OrderBy(input.Sorting ?? "Id desc");

            var facilities = await shipperFacilities.PageBy(input).ToListAsync();
            var totalCount = await shipperFacilities.CountAsync();
            return new PagedResultDto<FacilityLocationListDto>() { Items = await ToFacilityLocationDto(facilities), TotalCount = totalCount };
        }


        public async Task<InvoicingInformationDto> GetInvoicingInformation(int tenantId)
        {

            var tenant = await TenantManager.GetByIdAsync(tenantId);

            var creditLimit = await FeatureChecker.GetValueAsync(tenantId,
                AppFeatures.ShipperCreditLimit);
            var currentBalance = tenant.Balance - tenant.ReservedBalance;

            var paymentMethodId = int.Parse(await FeatureChecker.GetValueAsync(tenantId,
                AppFeatures.InvoicePaymentMethod));
            var paymentMethod = await _lookupPaymentMethodRepository.FirstOrDefaultAsync(paymentMethodId);

            return new InvoicingInformationDto()
            {
                CreditLimit = creditLimit,
                CreditType = paymentMethod.DisplayName,
                CurrentBalance = currentBalance.ToString(CultureInfo.CurrentUICulture),
                InvoicingDuePeriod = paymentMethod.InvoiceDueDateDays
            };
        }

        #endregion

        #region CarrierServicesOnly


        public async Task<FleetInformationDto> GetFleetInformation(GetFleetInformationInputDto input)
        {


            var translationQuery = _trucksTypesTranslationRepository
                .GetAll()
                .Where(i => i.Language.Contains(CultureInfo.CurrentUICulture.Name));

            var resultQuery = from t in _lookupTruckRepository.GetAll()
                              join r in translationQuery.DefaultIfEmpty() on t.TrucksTypeId equals r.CoreId
                              select new
                              {
                                  r.CoreId,
                                  r.TranslatedDisplayName,
                              };

            var availableTrucks = resultQuery
               .GroupBy(x => new { TrucksTypeId = x.CoreId, x.TranslatedDisplayName })
               .Select(g => new TruckTypeAvailableTrucksDto
               {
                   Id = g.Key.TrucksTypeId,
                   AvailableTrucksCount = g.Count(),
                   TruckType = g.Key.TranslatedDisplayName

               });


            //var availableTrucks = 

            var pageResult = await availableTrucks.PageBy(input).ToListAsync();
            var totalCount = await availableTrucks.CountAsync();
            var driversCount = await _lookupUserRepository.CountAsync(x => x.TenantId == input.TenantId && x.IsDriver);

            return new FleetInformationDto()
            {
                AvailableTrucksDto = new PagedResultDto<TruckTypeAvailableTrucksDto>()
                { Items = pageResult, TotalCount = totalCount },
                TotalDrivers = driversCount
            };
        }


        public async Task<PagedResultDto<AvailableVasDto>> GetAvailableVases(GetAvailableVasesInputDto input)
        {
            // Ask if Need (Domain Service)
            var availableVases = _lookupVasPriceRepository.GetAll()
                .Include(x => x.VasFk)
                .ThenInclude(x => x.Translations)
                .Where(x => x.TenantId == input.CarrierTenantId && !x.VasFk.IsDeleted)
                .OrderBy(input.Sorting ?? "Id desc");

            var pageResult = await availableVases.PageBy(input).ToListAsync();
            var totalCount = await availableVases.CountAsync();

            return new PagedResultDto<AvailableVasDto>() { Items = ObjectMapper.Map<List<AvailableVasDto>>(pageResult), TotalCount = totalCount };
        }

        #endregion

        public async Task UpdateCurrentUserProfile(CurrentUserProfileEditDto input)
        {
            var user = await GetCurrentUserAsync();

            if (user.PhoneNumber != input.PhoneNumber)
            {
                input.IsPhoneNumberConfirmed = false;
            }
            else if (user.IsPhoneNumberConfirmed)
            {
                input.IsPhoneNumberConfirmed = true;
            }

            ObjectMapper.Map(input, user);
            CheckErrors(await UserManager.UpdateAsync(user));

            if (Clock.SupportsMultipleTimezone)
            {
                if (input.Timezone.IsNullOrEmpty())
                {
                    var defaultValue =
                        await _timeZoneService.GetDefaultTimezoneAsync(SettingScopes.User, AbpSession.TenantId);
                    await SettingManager.ChangeSettingForUserAsync(AbpSession.ToUserIdentifier(),
                        TimingSettingNames.TimeZone, defaultValue);
                }
                else
                {
                    await SettingManager.ChangeSettingForUserAsync(AbpSession.ToUserIdentifier(),
                        TimingSettingNames.TimeZone, input.Timezone);
                }
            }
        }

        public async Task ChangePassword(ChangePasswordInput input)
        {
            await UserManager.InitializeOptionsAsync(AbpSession.TenantId);

            var user = await GetCurrentUserAsync();
            if (await UserManager.CheckPasswordAsync(user, input.CurrentPassword))
            {
                CheckErrors(await UserManager.ChangePasswordAsync(user, input.NewPassword));
            }
            else
            {
                CheckErrors(IdentityResult.Failed(new IdentityError
                {
                    Description = "Incorrect password."
                }));
            }
        }

        public async Task UpdateProfilePicture(UpdateProfilePictureInput input)
        {
            await SettingManager.ChangeSettingForUserAsync(
                AbpSession.ToUserIdentifier(),
                AppSettings.UserManagement.UseGravatarProfilePicture,
                input.UseGravatarProfilePicture.ToString().ToLowerInvariant()
            );

            if (input.UseGravatarProfilePicture)
            {
                return;
            }

            byte[] byteArray;

            var imageBytes = _tempFileCacheManager.GetFile(input.FileToken);

            if (imageBytes == null)
            {
                throw new UserFriendlyException("There is no such image file with the token: " + input.FileToken);
            }

            using (var bmpImage = new Bitmap(new MemoryStream(imageBytes)))
            {
                var width = (input.Width == 0 || input.Width > bmpImage.Width) ? bmpImage.Width : input.Width;
                var height = (input.Height == 0 || input.Height > bmpImage.Height) ? bmpImage.Height : input.Height;
                var bmCrop = bmpImage.Clone(new Rectangle(input.X, input.Y, width, height), bmpImage.PixelFormat);

                using (var stream = new MemoryStream())
                {
                    bmCrop.Save(stream, bmpImage.RawFormat);
                    byteArray = stream.ToArray();
                }
            }

            if (byteArray.Length > MaxProfilPictureBytes)
            {
                throw new UserFriendlyException(L("ResizedProfilePicture_Warn_SizeLimit",
                    AppConsts.ResizedMaxProfilPictureBytesUserFriendlyValue));
            }

            var user = await UserManager.GetUserByIdAsync(AbpSession.GetUserId());

            if (user.ProfilePictureId.HasValue)
            {
                await _binaryObjectManager.DeleteAsync(user.ProfilePictureId.Value);
            }

            var storedFile = new BinaryObject(AbpSession.TenantId, byteArray);
            await _binaryObjectManager.SaveAsync(storedFile);

            user.ProfilePictureId = storedFile.Id;
        }

        [AbpAllowAnonymous]
        public async Task<GetPasswordComplexitySettingOutput> GetPasswordComplexitySetting()
        {
            var passwordComplexitySetting = new PasswordComplexitySetting
            {
                RequireDigit =
                    await SettingManager.GetSettingValueAsync<bool>(AbpZeroSettingNames.UserManagement
                        .PasswordComplexity.RequireDigit),
                RequireLowercase =
                    await SettingManager.GetSettingValueAsync<bool>(AbpZeroSettingNames.UserManagement
                        .PasswordComplexity.RequireLowercase),
                RequireNonAlphanumeric =
                    await SettingManager.GetSettingValueAsync<bool>(AbpZeroSettingNames.UserManagement
                        .PasswordComplexity.RequireNonAlphanumeric),
                RequireUppercase =
                    await SettingManager.GetSettingValueAsync<bool>(AbpZeroSettingNames.UserManagement
                        .PasswordComplexity.RequireUppercase),
                RequiredLength =
                    await SettingManager.GetSettingValueAsync<int>(AbpZeroSettingNames.UserManagement.PasswordComplexity
                        .RequiredLength)
            };

            return new GetPasswordComplexitySettingOutput
            {
                Setting = passwordComplexitySetting
            };
        }

        [DisableAuditing]
        public async Task<GetProfilePictureOutput> GetProfilePicture(long? userId)
        {
            UserIdentifier userIdentifier;
            if (!(userId is null))
            {
                var tenantId = await (from user in _lookupUserRepository.GetAll()
                                      where user.Id == userId
                                      select user.TenantId).FirstOrDefaultAsync();

                userIdentifier = new UserIdentifier(tenantId, userId.Value);
            }
            else userIdentifier = AbpSession.ToUserIdentifier();

            using (var profileImageService = await _profileImageServiceFactory.Get(userIdentifier))
            {
                var profilePictureContent = await profileImageService.Object.GetProfilePictureContentForUser(
                    AbpSession.ToUserIdentifier()
                );

                return new GetProfilePictureOutput(profilePictureContent);
            }
        }

        [AbpAllowAnonymous]
        public async Task<GetProfilePictureOutput> GetProfilePictureByUserName(string username)
        {
            var user = await UserManager.FindByNameAsync(username);
            if (user == null)
            {
                return new GetProfilePictureOutput(string.Empty);
            }

            var userIdentifier = new UserIdentifier(AbpSession.TenantId, user.Id);
            using (var profileImageService = await _profileImageServiceFactory.Get(userIdentifier))
            {
                var profileImage = await profileImageService.Object.GetProfilePictureContentForUser(userIdentifier);
                return new GetProfilePictureOutput(profileImage);
            }
        }

        public async Task<GetProfilePictureOutput> GetFriendProfilePicture(GetFriendProfilePictureInput input)
        {
            var friendUserIdentifier = input.ToUserIdentifier();
            var friendShip = await _friendshipManager.GetFriendshipOrNullAsync(
                AbpSession.ToUserIdentifier(),
                friendUserIdentifier
            );

            if (friendShip == null)
            {
                return new GetProfilePictureOutput(string.Empty);
            }


            using (var profileImageService = await _profileImageServiceFactory.Get(friendUserIdentifier))
            {
                var image = await profileImageService.Object.GetProfilePictureContentForUser(friendUserIdentifier);
                return new GetProfilePictureOutput(image);
            }
        }

        [AbpAllowAnonymous]
        public async Task<GetProfilePictureOutput> GetProfilePictureByUser(long userId)
        {
            var userIdentifier = new UserIdentifier(AbpSession.TenantId, userId);
            using (var profileImageService = await _profileImageServiceFactory.Get(userIdentifier))
            {
                var profileImage = await profileImageService.Object.GetProfilePictureContentForUser(userIdentifier);
                return new GetProfilePictureOutput(profileImage);
            }
        }

        public async Task ChangeLanguage(ChangeUserLanguageDto input)
        {
            await SettingManager.ChangeSettingForUserAsync(
                AbpSession.ToUserIdentifier(),
                LocalizationSettingNames.DefaultLanguage,
                input.LanguageName
            );
        }

        public async Task<bool> IsProfileCompleted(int tenantId)
        {
            var tenant = await TenantManager.GetByIdAsync(tenantId);
            return (!tenant.Description.IsNullOrEmpty() && !tenant.Website.IsNullOrEmpty());
        }
        private async Task<byte[]> GetProfilePictureByIdOrNull(Guid profilePictureId)
        {
            var file = await _binaryObjectManager.GetOrNullAsync(profilePictureId);
            if (file == null)
            {
                return null;
            }

            return file.Bytes;
        }

        private async Task<GetProfilePictureOutput> GetProfilePictureByIdInternal(Guid profilePictureId)
        {
            var bytes = await GetProfilePictureByIdOrNull(profilePictureId);
            if (bytes == null)
            {
                return new GetProfilePictureOutput(string.Empty);
            }

            return new GetProfilePictureOutput(Convert.ToBase64String(bytes));
        }

        private async Task<TenantProfileInformationDto> GetTenantProfileInformation(int tenantId)
        {
            var tenant = await TenantManager.GetByIdAsync(tenantId);
            var profileInformation = ObjectMapper.Map<TenantProfileInformationDto>(tenant);
            profileInformation.CompanyEmailAddress = await GetCompanyEmailAddress(tenantId);
            profileInformation.Rating = tenant.Rate;
            return profileInformation;
        }

        private async Task<string> GetCityNameAsync(int cityId)
        {
            var cityName = await (from city in _lookupCityRepository.GetAll()
                                  where city.Id == cityId
                                  select city.Translations.FirstOrDefault(x => x.Language.Contains(CultureInfo.CurrentUICulture.Name)) != null ?
                                      city.Translations.FirstOrDefault(x => x.Language.Contains(CultureInfo.CurrentUICulture.Name)).TranslatedDisplayName
                                  : city.DisplayName).FirstOrDefaultAsync();
            return cityName;
        }

        private async Task<string> GetCountryNameAsync(int countryId)
        {
            var countryName = await (from city in _lookupCityRepository.GetAll()
                                     where city.CountyId == countryId
                                     select city.CountyFk.Translations.FirstOrDefault(x =>
                                         x.Language.Contains(CultureInfo.CurrentUICulture.Name)) != null
                                         ? city.CountyFk.Translations.FirstOrDefault(x => x.Language.Contains(CultureInfo.CurrentUICulture.Name))
                                             .TranslatedDisplayName : city.CountyFk.DisplayName).FirstOrDefaultAsync();
            return countryName;
        }
        private async Task<String> GetCompanyEmailAddress(int tenantId)
        {
            DisableTenancyFilters();
            return await (from user in _lookupUserRepository.GetAll()
                          where user.TenantId == tenantId && user.UserName == AbpUserBase.AdminUserName
                          select user.EmailAddress).FirstOrDefaultAsync();
        }

        private async Task<List<FacilityLocationListDto>> ToFacilityLocationDto(List<Facility> facilities)
        {
            var pageResult = new List<FacilityLocationListDto>();

            for (var i = 0; i < facilities.Count; i++)
            {
                var facility = facilities.ElementAt(i);
                var dto = ObjectMapper.Map<FacilityLocationListDto>(facility);
                dto.CityName = await GetCityNameAsync(facility.CityId);
                dto.CountryName = await GetCityNameAsync(facility.CityFk.CountyId);
                pageResult.Add(dto);
            }

            return pageResult;
        }

    }
}