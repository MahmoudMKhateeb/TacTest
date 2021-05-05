using Abp.Application.Features;
using Abp.Application.Services.Dto;
using Abp.Authorization.Users;
using Abp.Configuration;
using Abp.Configuration.Startup;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Localization;
using Abp.Runtime.Session;
using Abp.Timing;
using Abp.UI;
using Abp.Zero.Configuration;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TACHYON.Cities;
using TACHYON.Configuration;
using TACHYON.Countries;
using TACHYON.Countries.Dtos;
using TACHYON.Debugging;
using TACHYON.Dto;
using TACHYON.Editions;
using TACHYON.Editions.Dto;
using TACHYON.Features;
using TACHYON.MultiTenancy.Dto;
using TACHYON.MultiTenancy.Payments;
using TACHYON.MultiTenancy.Payments.Dto;
using TACHYON.Notifications;
using TACHYON.Security.Recaptcha;
using TACHYON.TermsAndConditions;
using TACHYON.TermsAndConditions.Dtos;
using TACHYON.Url;

namespace TACHYON.MultiTenancy
{
    public class TenantRegistrationAppService : TACHYONAppServiceBase, ITenantRegistrationAppService
    {
        public IAppUrlService AppUrlService { get; set; }

        private readonly IMultiTenancyConfig _multiTenancyConfig;
        private readonly IRecaptchaValidator _recaptchaValidator;
        private readonly EditionManager _editionManager;
        private readonly IAppNotifier _appNotifier;
        private readonly ILocalizationContext _localizationContext;
        private readonly TenantManager _tenantManager;
        private readonly ISubscriptionPaymentRepository _subscriptionPaymentRepository;
        private readonly IRepository<County, int> _lookup_countryRepository;
        private readonly IRepository<City, int> _lookup_cityRepository;
        private readonly IRepository<TermAndCondition> _termAndConditionRepository;

        public TenantRegistrationAppService(
            IMultiTenancyConfig multiTenancyConfig,
            IRecaptchaValidator recaptchaValidator,
            EditionManager editionManager,
            IAppNotifier appNotifier,
            ILocalizationContext localizationContext,
            TenantManager tenantManager,
            IRepository<County, int> lookup_countryRepository,
            IRepository<City, int> lookup_cityRepository,
            IRepository<TermAndCondition> termAndConditionRepository,
            ISubscriptionPaymentRepository subscriptionPaymentRepository)
        {
            _multiTenancyConfig = multiTenancyConfig;
            _recaptchaValidator = recaptchaValidator;
            _editionManager = editionManager;
            _appNotifier = appNotifier;
            _localizationContext = localizationContext;
            _tenantManager = tenantManager;
            _subscriptionPaymentRepository = subscriptionPaymentRepository;
            _lookup_countryRepository = lookup_countryRepository;
            _lookup_cityRepository = lookup_cityRepository;
            _termAndConditionRepository = termAndConditionRepository;

            AppUrlService = NullAppUrlService.Instance;
        }

        public async Task<RegisterTenantOutput> RegisterTenant(RegisterTenantInput input)
        {
            if (input.EditionId.HasValue)
            {
                await CheckEditionSubscriptionAsync(input.EditionId.Value, input.SubscriptionStartType);
            }
            else
            {
                await CheckRegistrationWithoutEdition();
            }

            using (CurrentUnitOfWork.SetTenantId(null))
            {
                var isMailValid = await CheckIfEmailisAvailable(input.AdminEmailAddress);

                var tenancyName = input.companyName.Trim().Replace(" ", "_");
                var isCompanyNameValid = CheckIfCompanyUniqueNameisAvailable(tenancyName);

                if (!isCompanyNameValid || !isMailValid)
                {
                    throw new Exception("admin Email Or CompanyName Is Already Taken!");
                }

                CheckTenantRegistrationIsEnabled();

                if (UseCaptchaOnRegistration())
                {
                    await _recaptchaValidator.ValidateAsync(input.CaptchaResponse);
                }

                //Getting host-specific settings
                var isActive = await IsNewRegisteredTenantActiveByDefault(input.SubscriptionStartType);
                //var isEmailConfirmationRequired = await SettingManager.GetSettingValueForApplicationAsync<bool>(
                //    AbpZeroSettingNames.UserManagement.IsEmailConfirmationRequiredForLogin
                //);

                //todo add setting here
                var isEmailConfirmationRequired = true;

                DateTime? subscriptionEndDate = null;
                var isInTrialPeriod = false;

                if (input.EditionId.HasValue)
                {
                    isInTrialPeriod = input.SubscriptionStartType == SubscriptionStartType.Trial;

                    if (isInTrialPeriod)
                    {
                        var edition = (SubscribableEdition)await _editionManager.GetByIdAsync(input.EditionId.Value);
                        subscriptionEndDate = Clock.Now.AddDays(edition.TrialDayCount ?? 0);
                    }
                }

                var tenantId = await _tenantManager.CreateWithAdminUserAsync(
                    input.companyName,
                    input.MobileNo,
                    tenancyName,
                    input.Name,
                    input.Address,
                    input.CityId,
                    input.CountryId,
                    input.AdminPassword,
                    input.AdminEmailAddress,
                    null,
                    isActive,
                    input.EditionId,
                    shouldChangePasswordOnNextLogin: false,
                    sendActivationEmail: true,
                    subscriptionEndDate,
                    isInTrialPeriod,
                    AppUrlService.CreateEmailActivationUrlFormat(tenancyName),
                    input.UserAdminFirstName,
                    input.UserAdminSurname

                );

                var tenant = await TenantManager.GetByIdAsync(tenantId);
                tenant.AccountNumber = long.Parse(string.Format("{0}{1}",Clock.Now.ToString("yy"), tenant.Id.ToString("D5")));
                await TenantManager.UpdateAsync(tenant);
                await _appNotifier.NewTenantRegisteredAsync(tenant);

                return new RegisterTenantOutput
                {
                    TenantId = tenant.Id,
                    TenancyName = tenancyName,
                    Name = input.Name,
                    UserName = AbpUserBase.AdminUserName,
                    EmailAddress = input.AdminEmailAddress,
                    IsActive = tenant.IsActive,
                    IsEmailConfirmationRequired = isEmailConfirmationRequired,
                    IsTenantActive = tenant.IsActive
                };
            }
        }

        private async Task<bool> IsNewRegisteredTenantActiveByDefault(SubscriptionStartType subscriptionStartType)
        {
            if (subscriptionStartType == SubscriptionStartType.Paid)
            {
                return false;
            }

            return await SettingManager.GetSettingValueForApplicationAsync<bool>(AppSettings.TenantManagement.IsNewRegisteredTenantActiveByDefault);
        }

        private async Task CheckRegistrationWithoutEdition()
        {
            var editions = await _editionManager.GetAllAsync();
            if (editions.Any())
            {
                throw new Exception("Tenant registration is not allowed without edition because there are editions defined !");
            }
        }

        public async Task<EditionsSelectOutput> GetEditionsForSelect()
        {
            var features = FeatureManager
                .GetAll()
                .Where(feature => (feature[FeatureMetadata.CustomFeatureKey] as FeatureMetadata)?.IsVisibleOnPricingTable ?? false);

            var flatFeatures = ObjectMapper
                .Map<List<FlatFeatureSelectDto>>(features)
                .OrderBy(f => f.DisplayName)
                .ToList();

            var editions = (await _editionManager.GetAllAsync())
                .Cast<SubscribableEdition>()
                .Where(x=>!x.DisplayName.ToLower().Contains(TACHYONConsts.TachyonDealerEdtionName.ToLower()))
                .OrderBy(e => e.MonthlyPrice)
                .ToList();

            var featureDictionary = features.ToDictionary(feature => feature.Name, f => f);

            var editionWithFeatures = new List<EditionWithFeaturesDto>();
            foreach (var edition in editions)
            {
                editionWithFeatures.Add(await CreateEditionWithFeaturesDto(edition, featureDictionary));
            }

            if (AbpSession.UserId.HasValue)
            {
                var currentEditionId = (await _tenantManager.GetByIdAsync(AbpSession.GetTenantId()))
                        .EditionId;

                if (currentEditionId.HasValue)
                {
                    editionWithFeatures = editionWithFeatures.Where(e => e.Edition.Id != currentEditionId).ToList();

                    var currentEdition = (SubscribableEdition)(await _editionManager.GetByIdAsync(currentEditionId.Value));
                    var lastPayment = await _subscriptionPaymentRepository.GetLastCompletedPaymentOrDefaultAsync(
                        AbpSession.GetTenantId(),
                        null,
                        null);

                    if (lastPayment != null)
                    {
                        editionWithFeatures = editionWithFeatures
                            .Where(e =>
                                e.Edition.GetPaymentAmount(lastPayment.PaymentPeriodType) >
                                currentEdition.GetPaymentAmount(lastPayment.PaymentPeriodType)
                            )
                            .ToList();
                    }
                }
            }

            return new EditionsSelectOutput
            {
                AllFeatures = flatFeatures,
                EditionsWithFeatures = editionWithFeatures,
            };
        }

        public async Task<EditionSelectDto> GetEdition(int editionId)
        {
            var edition = await _editionManager.GetByIdAsync(editionId);
            var editionDto = ObjectMapper.Map<EditionSelectDto>(edition);

            return editionDto;
        }

        private async Task<EditionWithFeaturesDto> CreateEditionWithFeaturesDto(SubscribableEdition edition, Dictionary<string, Feature> featureDictionary)
        {
            return new EditionWithFeaturesDto
            {
                Edition = ObjectMapper.Map<EditionSelectDto>(edition),
                FeatureValues = (await _editionManager.GetFeatureValuesAsync(edition.Id))
                    .Where(featureValue => featureDictionary.ContainsKey(featureValue.Name))
                    .Select(fv => new NameValueDto(
                        fv.Name,
                        featureDictionary[fv.Name].GetValueText(fv.Value, _localizationContext))
                    )
                    .ToList()
            };
        }

        private void CheckTenantRegistrationIsEnabled()
        {
            if (!IsSelfRegistrationEnabled())
            {
                throw new UserFriendlyException(L("SelfTenantRegistrationIsDisabledMessage_Detail"));
            }

            if (!_multiTenancyConfig.IsEnabled)
            {
                throw new UserFriendlyException(L("MultiTenancyIsNotEnabled"));
            }
        }

        private bool IsSelfRegistrationEnabled()
        {
            return SettingManager.GetSettingValueForApplication<bool>(AppSettings.TenantManagement.AllowSelfRegistration);
        }

        private bool UseCaptchaOnRegistration()
        {
            return SettingManager.GetSettingValueForApplication<bool>(AppSettings.TenantManagement.UseCaptchaOnRegistration);
        }

        private async Task CheckEditionSubscriptionAsync(int editionId, SubscriptionStartType subscriptionStartType)
        {
            var edition = await _editionManager.GetByIdAsync(editionId) as SubscribableEdition;

            CheckSubscriptionStart(edition, subscriptionStartType);
        }

        private static void CheckSubscriptionStart(SubscribableEdition edition, SubscriptionStartType subscriptionStartType)
        {
            switch (subscriptionStartType)
            {
                case SubscriptionStartType.Free:
                    if (!edition.IsFree)
                    {
                        throw new Exception("This is not a free edition !");
                    }
                    break;
                case SubscriptionStartType.Trial:
                    if (!edition.HasTrial())
                    {
                        throw new Exception("Trial is not available for this edition !");
                    }
                    break;
                case SubscriptionStartType.Paid:
                    if (edition.IsFree)
                    {
                        throw new Exception("This is a free edition and cannot be subscribed as paid !");
                    }
                    break;
            }
        }


        public async Task<List<TenantCountryLookupTableDto>> GetAllCountryForTableDropdown()
        {
            List<County> countries = await _lookup_countryRepository
                .GetAllIncluding(x => x.Translations)
                .OrderBy(x => x.DisplayName)
                .ToListAsync();

            List<TenantCountryLookupTableDto> countryDtos = ObjectMapper.Map<List<TenantCountryLookupTableDto>>(countries);
            return countryDtos;
        }

        public async Task<List<CountyDto>> GetAllCountriesWithCode()
        {
            var countries = await _lookup_countryRepository
                .GetAll().OrderBy(x => x.DisplayName)
                .ToListAsync();
            var result = ObjectMapper.Map<List<CountyDto>>(countries);
            return result;
        }
        public async Task<List<TenantCityLookupTableDto>> GetAllCitiesForTableDropdown(int input)
        {
            List<City> cities = await _lookup_cityRepository
                .GetAllIncluding(x => x.Translations)
                .Where(x => x.CountyFk.Id == input)
                .OrderBy(x => x.DisplayName)
                .ToListAsync();

            List<TenantCityLookupTableDto> cityDtos = ObjectMapper.Map<List<TenantCityLookupTableDto>>(cities);
            return cityDtos;

        }



        public bool CheckIfCompanyUniqueNameisAvailable(string CompanyName)
        {
            using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MayHaveTenant))
            {
                var result = _tenantManager.FindByTenancyName(CompanyName);
                if (result == null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        public async Task<bool> CheckIfEmailisAvailable(string email)
        {
            using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MayHaveTenant))
            {
                var result = await UserManager.FindByEmailAsync(email == null ? "" : email);
                if (result == null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

        }

        public async Task<GetTermAndConditionForViewDto> GetActiveTermAndConditionForViewAndApprove(string editiontId)
        {
            using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MayHaveTenant, AbpDataFilters.MustHaveTenant))
            {
                var term = await _termAndConditionRepository.GetAll()
                .Include(x => x.EditionFk)
                .Include(x => x.Translations)
                .FirstOrDefaultAsync(x => x.EditionId == int.Parse(editiontId));
                var output = new GetTermAndConditionForViewDto
                {
                    TermAndCondition = ObjectMapper.Map<TermAndConditionDto>(term)
                };
                return output;
            }

        }
    }
}