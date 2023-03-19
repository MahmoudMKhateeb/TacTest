using Abp.Application.Features;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Authorization.Users;
using Abp.Collections.Extensions;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Linq.Extensions;
using Abp.UI;
using AutoMapper.QueryableExtensions;
using DevExpress.XtraRichEdit.Model;
using DevExtreme.AspNet.Data.ResponseModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using NUglify.Helpers;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using TACHYON.Authorization;
using TACHYON.Authorization.Users;
using TACHYON.Common;
using TACHYON.Dto;
using TACHYON.Features;
using TACHYON.MultiTenancy;
using TACHYON.PriceOffers;
using TACHYON.PriceOffers.Dto;
using TACHYON.PricePackages.Dto;
using TACHYON.PricePackages.PricePackageOffers;
using TACHYON.ServiceAreas;
using TACHYON.Shipping.DirectRequests;
using TACHYON.Shipping.ShippingRequests;
using TACHYON.Trucks.TruckCategories.TransportTypes;
using TACHYON.Trucks.TrucksTypes;

namespace TACHYON.PricePackages
{ 
    [AbpAuthorize(AppPermissions.Pages_PricePackages)]
    public class PricePackageAppService : TACHYONAppServiceBase, IPricePackageAppService
    {
        private readonly IRepository<PricePackage,long> _pricePackageRepository;
        private readonly IRepository<Tenant> _tenantRepository;
        private readonly IRepository<ShippingRequest, long> _shippingRequestRepository;
        private readonly IRepository<ShippingRequestDirectRequest, long> _directRequestRepository;
        private readonly IPricePackageManager _pricePackageManager;
        private readonly IPricePackageOfferManager _pricePackageOfferManager;
        private readonly IRepository<PricePackageOffer,long> _pricePackageOfferRepository;
        private readonly IRepository<PriceOffer,long> _priceOfferRepository;
        private readonly IPriceOfferAppService _priceOfferAppService;
        private readonly IRepository<User,long> _userRepository;
        private readonly IRepository<TrucksType, long> _truckTypeRepository;
        private readonly IRepository<TransportType> _transportTypeRepository;
        private readonly IShippingRequestDirectRequestAppService _directRequestAppService;

        public PricePackageAppService(
            IRepository<PricePackage,long> pricePackageRepository,
            IRepository<Tenant> tenantRepository,
            IRepository<ShippingRequest, long> shippingRequestRepository,
            IPricePackageManager pricePackageManager,
            IRepository<ShippingRequestDirectRequest, long> directRequestRepository,
            IRepository<PriceOffer, long> priceOfferRepository,
            IRepository<User, long> userRepository,
            IPriceOfferAppService priceOfferAppService,
            IRepository<PricePackageOffer, long> pricePackageOfferRepository,
            IPricePackageOfferManager pricePackageOfferManager, 
            IRepository<TrucksType, long> truckTypeRepository,
            IRepository<TransportType> transportTypeRepository,
            IShippingRequestDirectRequestAppService directRequestAppService)
        {
            _pricePackageRepository = pricePackageRepository;
            _tenantRepository = tenantRepository;
            _shippingRequestRepository = shippingRequestRepository;
            _pricePackageManager = pricePackageManager;
            _directRequestRepository = directRequestRepository;
            _priceOfferRepository = priceOfferRepository;
            _userRepository = userRepository;
            _priceOfferAppService = priceOfferAppService;
            _pricePackageOfferRepository = pricePackageOfferRepository;
            _pricePackageOfferManager = pricePackageOfferManager;
            _truckTypeRepository = truckTypeRepository;
            _transportTypeRepository = transportTypeRepository;
            _directRequestAppService = directRequestAppService;
        }


        public async Task<LoadResult> GetAll(LoadOptionsInput input)
        {
            DisableTenancyFilters();
            var isTmsOrHost = !AbpSession.TenantId.HasValue || await IsTachyonDealer();
            var pricePackages = _pricePackageRepository.GetAllIncluding(x=> x.ServiceAreas).AsNoTracking()
                .WhereIf(!isTmsOrHost,
                    x => (x.UsageType == PricePackageUsageType.AsTachyonManageService &&
                          (x.DestinationTenantId == AbpSession.TenantId &&
                           (x.Proposal.Appendix.Status == AppendixStatus.Confirmed ||
                            x.Appendix.Status == AppendixStatus.Confirmed) &&
                           (x.Proposal.Appendix.IsActive || x.Appendix.IsActive))) ||
                         (x.UsageType == PricePackageUsageType.AsCarrier && x.TenantId == AbpSession.TenantId))
                .ProjectTo<PricePackageListDto>(AutoMapperConfigurationProvider);

            return await LoadResultAsync(pricePackages, input.LoadOptions);
        }

        public async Task<PricePackageForViewDto> GetForView(long pricePackageId)
        {
            var pricePackage = await _pricePackageRepository.GetAll().AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == pricePackageId);

            if (pricePackage == null) throw new EntityNotFoundException(L("NotFound"));
            
            return ObjectMapper.Map<PricePackageForViewDto>(pricePackage);
        }

        
        public async Task CreateOrEdit(CreateOrEditPricePackageDto input)
        {
            
            await ValidateUsageType(input);


            if (input.Id.HasValue)
            {
                await Update(input);
                return;
            }

            await Create(input);

            async Task ValidateUsageType(CreateOrEditPricePackageDto createOrEditPricePackageDto)
            {
                bool isTms = await IsTachyonDealer();
                if (isTms)
                {
                    if (!createOrEditPricePackageDto.UsageType.HasValue)
                        throw new UserFriendlyException(L("YouMustProvideTheUsageType"));
                }
                else
                {
                    if (createOrEditPricePackageDto.UsageType.HasValue)
                        throw new UserFriendlyException(L("YouCanNotChangePricePackageUsageType"));

                    createOrEditPricePackageDto.UsageType = PricePackageUsageType.AsCarrier;
                }
            }
            
        }

        [AbpAuthorize(AppPermissions.Pages_PricePackages_Create)]
        protected virtual async Task Create(CreateOrEditPricePackageDto input)
        {
            var createdPricePackage = ObjectMapper.Map<PricePackage>(input);

            await CheckPricePackageImpersonate(createdPricePackage); 
            await _pricePackageRepository.InsertAsync(createdPricePackage);

            createdPricePackage.PricePackageReference =
                _pricePackageManager.GeneratePricePackageReferenceNumber(createdPricePackage);
        }

        private async Task CheckPricePackageImpersonate(PricePackage pricePackage)
        {
            if (!AbpSession.TenantId.HasValue || await IsTachyonDealer())
            {
                switch (pricePackage.UsageType)
                {
                    case PricePackageUsageType.AsCarrier when !pricePackage.DestinationTenantId.HasValue:
                        throw new UserFriendlyException(L("YouMustSelectACompany"));
                    case PricePackageUsageType.AsCarrier:
                        pricePackage.TenantId = pricePackage.DestinationTenantId.Value;
                        pricePackage.DestinationTenantId = default; // default means null
                        break;
                    case PricePackageUsageType.AsTachyonManageService when !AbpSession.TenantId.HasValue:
                        pricePackage.TenantId = await _tenantRepository.GetAll().Where(x => x.Edition.Id == TachyonEditionId).Select(x => x.Id)
                            .FirstOrDefaultAsync();
                        if (!pricePackage.ServiceAreas.IsNullOrEmpty())
                        {
                            pricePackage.ServiceAreas.ForEach(x=> x.TenantId = pricePackage.TenantId);
                        }
                        break;
                }
            }

            
        }

        [AbpAuthorize(AppPermissions.Pages_PricePackages_Update)]
        protected virtual async Task Update(CreateOrEditPricePackageDto input)
        {
            if (!input.Id.HasValue) return;
            var updatedPricePackage = await _pricePackageRepository.GetAllIncluding(x => x.ServiceAreas)
                .FirstOrDefaultAsync(x => x.Id == input.Id.Value);
            if (updatedPricePackage.Type != input.Type)
                throw new UserFriendlyException(L("YouCanNotChangeThePricePackageType"));

            if (updatedPricePackage.UsageType != input.UsageType)
                throw new UserFriendlyException(L("YouCanNotChangeTheUsageType"));

            ObjectMapper.Map(input, updatedPricePackage);
            await CheckPricePackageImpersonate(updatedPricePackage);
        }
        
        [AbpAuthorize(AppPermissions.Pages_PricePackages_Update)]
        public async Task<CreateOrEditPricePackageDto> GetForEdit(long pricePackageId)
        {
            DisableTenancyFilterIfTachyonDealerOrHost();
            
            var pricePackage = await _pricePackageRepository.GetAllIncluding(x=> x.ServiceAreas).AsNoTracking()
                .FirstOrDefaultAsync(x=> x.Id == pricePackageId);
            
            if (pricePackage == null) throw new EntityNotFoundException(L("NotFound"));
            
            return ObjectMapper.Map<CreateOrEditPricePackageDto>(pricePackage);
        }

        [AbpAuthorize(AppPermissions.Pages_PricePackages_Delete)]
        public async Task Delete(long pricePackageId)
        {
            var isExist = await _pricePackageRepository.GetAll().AnyAsync(x => x.Id == pricePackageId);

            if (!isExist) throw new EntityNotFoundException(L("NotFound"));

            await _pricePackageRepository.DeleteAsync(x => x.Id == pricePackageId);
        }

        public async Task ApplyPricePackage(long pricePackageId,long srId)
        {
            await _pricePackageOfferManager.ApplyPricingOnShippingRequest(pricePackageId, srId);
        }

        public async Task<PricePackageForPricingDto> GetForPricing(long pricePackageId)
        {
            DisableTenancyFilters();
            var pricePackage = await _pricePackageRepository.GetAll().AsNoTracking()
                .Where(x => x.Id == pricePackageId)
                .WhereIf(AbpSession.TenantId.HasValue && !await IsTachyonDealer(),x=> x.DestinationTenantId == AbpSession.TenantId)
                .ProjectTo<PricePackageForPricingDto>(AutoMapperConfigurationProvider)
                .SingleAsync();
            return pricePackage;
        }

        public async Task<PagedResultDto<PricePackageForViewDto>> GetMatchingPricePackages(
            GetMatchingPricePackagesInput input)
        {
            DisableTenancyFilters();

            var isTmsOrHost = !AbpSession.TenantId.HasValue || await IsTachyonDealer();
            var matchedPricePackages = (from shippingRequest in _shippingRequestRepository.GetAll()
                    where shippingRequest.Id == input.ShippingRequestId
                    from pricePackage in _pricePackageRepository.GetAll().AsNoTracking()
                    let hasNormalDirectRequest = _directRequestRepository.GetAll().Any(x =>
                        x.ShippingRequestId == input.ShippingRequestId &&
                        x.CarrierTenantId == pricePackage.DestinationTenantId)
                    let ppOffer = (from pricePackageOffer in _pricePackageOfferRepository.GetAll()
                            .AsNoTracking()
                        where hasNormalDirectRequest ||
                              (pricePackageOffer != null && (pricePackageOffer.PricePackageId == pricePackage.Id) &&
                               (pricePackageOffer.DirectRequestId == null ||
                                pricePackageOffer.DirectRequest.ShippingRequestId == input.ShippingRequestId) &&
                               (pricePackageOffer.PriceOfferId == null ||
                                pricePackageOffer.PriceOffer.ShippingRequestId == input.ShippingRequestId))
                        select pricePackageOffer).FirstOrDefault()
                    where (pricePackage.ProposalId.HasValue || pricePackage.AppendixId.HasValue) &&
                          (!pricePackage.ProposalId.HasValue ||
                           (pricePackage.Proposal.Status == ProposalStatus.Approved &&
                            pricePackage.Proposal.AppendixId.HasValue &&
                            pricePackage.Proposal.Appendix.Status == AppendixStatus.Confirmed &&
                            pricePackage.Proposal.Appendix.IsActive &&
                            pricePackage.Proposal.ShipperId == shippingRequest.TenantId)) &&
                          (!pricePackage.AppendixId.HasValue ||
                           (pricePackage.Appendix.Status == AppendixStatus.Confirmed &&
                            pricePackage.Appendix.IsActive &&
                            !pricePackage.ProposalId.HasValue)) &&
                          (isTmsOrHost || pricePackage.DestinationTenantId == AbpSession.TenantId) &&
                          pricePackage.Type == PricePackageType.PerTrip &&
                          pricePackage.TruckTypeId == shippingRequest.TrucksTypeId &&
                          pricePackage.OriginCityId == shippingRequest.OriginCityId &&
                          pricePackage.RouteType == shippingRequest.RouteTypeId
                          && shippingRequest.ShippingRequestDestinationCities.Any(i =>
                              i.CityId == pricePackage.DestinationCityId)
                    orderby pricePackage.Id
                    select new PricePackageLookup
                    {
                        PricePackage = pricePackage,
                        HasDirectRequest = ppOffer.DirectRequestId.HasValue || hasNormalDirectRequest,
                        HasOffer = ppOffer.PriceOfferId.HasValue,
                        HasParentOffer = _pricePackageOfferManager
                            .GetParentOfferIdAsQueryable(input.ShippingRequestId).Any()
                    })
                .ProjectTo<PricePackageForViewDto>(AutoMapperConfigurationProvider);

            var pageResult = await matchedPricePackages.PageBy(input).ToListAsync();


            return new PagedResultDto<PricePackageForViewDto>()
            {
                Items = pageResult, TotalCount = await matchedPricePackages.CountAsync()
            };
        }

        public async Task<LoadResult> GetAppendixPricePackages(LoadOptionsInput input)
        {
            DisableTenancyFilters();
            var isTmsOrHost = !AbpSession.TenantId.HasValue || await IsTachyonDealer();
            var tmsPricePackages =  _pricePackageRepository.GetAll().AsNoTracking()
                .WhereIf(!isTmsOrHost,
                    x => x.DestinationTenantId == AbpSession.TenantId &&
                         (x.Proposal.Appendix.Status == AppendixStatus.Confirmed ||
                          x.Appendix.Status == AppendixStatus.Confirmed) && (x.Proposal.Appendix.IsActive || x.Appendix.IsActive))
                .ProjectTo<PricePackageForViewDto>(AutoMapperConfigurationProvider);

            return await LoadResultAsync(tmsPricePackages, input.LoadOptions); 
        }

        [RequiresFeature(AppFeatures.TachyonDealer)]
        public async Task<List<SelectItemDto>> GetCompanies(CompanyType companyType)
        {
            DisableTenancyFilters();
            if (companyType is not CompanyType.Shipper and not CompanyType.Carrier)
                throw new UserFriendlyException(L("PleaseSelectCompanyTypeShipperOrCarrier"));
            
            return await (from tenant in _tenantRepository.GetAll()
                    where (companyType == CompanyType.Carrier && tenant.EditionId == CarrierEditionId) ||
                          (companyType == CompanyType.Shipper && tenant.EditionId == ShipperEditionId)
                    select new SelectItemDto { DisplayName = tenant.Name, Id = tenant.Id.ToString() })
                .ToListAsync();
        }

        
        public async Task<LoadResult> GetAllForDropdown(GetPricePackagesInput input)
        {
            await DisableTenancyFilterIfTachyonDealerOrHost();
            var tmsPricePackages = _pricePackageRepository.GetAll()
                .AsNoTracking().Where(x=> x.DestinationTenantId == input.DestinationTenantId)
                .WhereIf(input.ProposalId.HasValue,x=> !x.ProposalId.HasValue || x.ProposalId == input.ProposalId)
                .WhereIf(!input.ProposalId.HasValue,x=> !x.ProposalId.HasValue)
                .ProjectTo<PricePackageSelectItemDto>(AutoMapperConfigurationProvider);

            return await LoadResultAsync(tmsPricePackages, input.LoadOptions);
        }
        [AbpAuthorize(AppPermissions.Pages_PricePackages_Create,AppPermissions.Pages_PricePackages_Update)]
        public async Task<ListResultDto<PricePackageSelectItemDto>> GetPricePackagesForCarrierAppendix(int carrierId, int? appendixId)
        {
            var pricePackagesList = await _pricePackageManager.GetPricePackagesForCarrierAppendix(carrierId, appendixId);

            return new ListResultDto<PricePackageSelectItemDto>(pricePackagesList);
        }

        public async Task<PricePackageForPriceCalculationDto> GetPricePackageOfferForHandle(long pricePackageId,
            long shippingRequestId)
        {
            return await _pricePackageOfferManager.GetPricePackageForPriceCalculation(pricePackageId, shippingRequestId);
        } 
        
        
        private async Task CreateOfferAndAcceptOnBehalfOfCarrier(long pricePackageId, long shippingRequestId)
        {
             // this method not ready yet to use
            int? carrierTenantId;
            long carrierUserId;
            
            using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MayHaveTenant, AbpDataFilters.MustHaveTenant))
            {
                var ppInfo = await (from pricePackage in _pricePackageRepository.GetAll()
                        where pricePackage.Id == pricePackageId
                        select new { pricePackage.DestinationTenantId, pricePackage.TenantId, pricePackage.UsageType })
                    .FirstAsync();
                carrierTenantId = ppInfo.UsageType == PricePackageUsageType.AsCarrier
                    ? ppInfo.TenantId
                    : ppInfo.DestinationTenantId;

                carrierUserId = await _userRepository.GetAll().Where(x =>
                        x.TenantId == carrierTenantId && x.UserName == AbpUserBase.AdminUserName)
                    .Select(x => x.Id).FirstOrDefaultAsync();
            }

            // impersonate the carrier 
            long createdOfferId;
            using (AbpSession.Use(carrierTenantId,carrierUserId))
            {
                var priceOfferDto = new PriceOfferDto(); // await _priceOfferAppService.GetPriceOfferForCreateOrEdit(shippingRequestId, null,null);

                
                var priceOffer = ObjectMapper.Map<PriceOffer>(priceOfferDto);

                priceOffer.ShippingRequestId = shippingRequestId;
                var priceOfferInput = ObjectMapper.Map<CreateOrEditPriceOfferInput>(priceOffer);

                priceOfferInput.Channel = PriceOfferChannel.DirectRequest;
                createdOfferId = await _priceOfferAppService.CreateOrEdit(priceOfferInput);
                _priceOfferRepository.Update(createdOfferId, x => x.CreatorUserId = carrierUserId);
            }

            await _priceOfferAppService.Accept(createdOfferId);
        }

        public async Task<long> SendPriceOfferByPricePackage(long pricePackageId, long shippingRequestId)
        {
            return await _pricePackageOfferManager.SendPriceOfferByPricePackage(pricePackageId, shippingRequestId);
        }
        
        public async Task HandlePricePackageOfferToCarrier(int pricePackageId, long shippingRequestId)
         {
             
             var directRequestInput = await _pricePackageOfferManager.GetDirectRequestToHandleByPricePackage(pricePackageId, shippingRequestId);
             await _directRequestAppService.Create(directRequestInput); 
         }

        public async Task<ShippingRequestDirectRequestStatus> AcceptPricePackageOffer(long pricePackageOfferId)
        { 
            return await _pricePackageOfferManager.AcceptPricePackageOffer(pricePackageOfferId);
        }

        #region LOOKUPs & Dropdowns

        public async Task<List<SelectItemDto>> GetAllTruckTypeForDropdown(int transportTypeId)
        {
            return await _truckTypeRepository.GetAll().AsNoTracking()
                .Where(c => c.TransportTypeId == transportTypeId)
                .Select(p => new SelectItemDto { DisplayName = p.DisplayName ?? "", Id = p.Id.ToString() })
                .ToListAsync();
        }

        public async Task<List<SelectItemDto>> GetAllTransportTypeForDropdown()
        {
            return await _transportTypeRepository.GetAllIncluding(x => x.Translations).AsNoTracking()
                .Select(p => new SelectItemDto
                {
                    DisplayName = p.Translations.Any(x => x.Language.Contains(CultureInfo.CurrentUICulture.Name))
                        ? p.Translations.FirstOrDefault(x => x.Language.Contains(CultureInfo.CurrentUICulture.Name))
                            .DisplayName
                        : p.DisplayName,
                    Id = p.Id.ToString()
                }).ToListAsync();
        }
        

        #endregion
    }
}