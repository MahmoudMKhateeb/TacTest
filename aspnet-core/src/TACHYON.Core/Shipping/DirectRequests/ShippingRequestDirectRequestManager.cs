using Abp.Application.Features;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Linq.Extensions;
using Abp.Runtime.Session;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TACHYON.Features;
using TACHYON.MultiTenancy;
using TACHYON.Notifications;
using TACHYON.PricePackages;
using TACHYON.Shipping.DirectRequests.Dto;
using TACHYON.Shipping.ShippingRequests;
using TACHYON.Shipping.ShippingRequests.Dtos;

namespace TACHYON.Shipping.DirectRequests
{
    public class ShippingRequestDirectRequestManager : TACHYONDomainServiceBase
    {
        private readonly IFeatureChecker _featureChecker;
        private readonly IRepository<Tenant> _tenantRepository;
        private readonly IRepository<TenantCarrier, long> _tenantCarrierRepository;
        private readonly IRepository<ShippingRequestDirectRequest, long> _shippingRequestDirectRequestRepository;
        private readonly IRepository<PricePackage, long> _pricePackageRepository;
        private readonly IRepository<ShippingRequest, long> _shippingRequestRepository;
        private readonly IAbpSession AbpSession;
        private readonly IAppNotifier _appNotifier;



        public ShippingRequestDirectRequestManager(IFeatureChecker featureChecker,
            IRepository<Tenant> tenantRepository,
            IRepository<TenantCarrier, long> tenantCarrierRepository,
            IRepository<ShippingRequestDirectRequest, long> shippingRequestDirectRequestRepository,
            IRepository<PricePackage, long> pricePackageRepository,
            IRepository<ShippingRequest, long> shippingRequestRepository,
            IAbpSession abpSession,
            IAppNotifier appNotifier)
        {
            _featureChecker = featureChecker;
            _tenantRepository = tenantRepository;
            _tenantCarrierRepository = tenantCarrierRepository;
            _shippingRequestDirectRequestRepository = shippingRequestDirectRequestRepository;
            _pricePackageRepository = pricePackageRepository;
            _shippingRequestRepository = shippingRequestRepository;
            AbpSession = abpSession;
            _appNotifier = appNotifier;
        }

        /// <summary>
        /// Get list of carrier for dropdown can send direct request
        /// </summary>
        /// <returns></returns>
        public async Task<List<CarriersForDropDownDto>> GetCarriersForDropDownByPermissionAsync()
        {
            List<CarriersForDropDownDto> list;
            if (await _featureChecker.IsEnabledAsync(AppFeatures.TachyonDealer))
            {
                list = await _tenantRepository
                    .GetAll()
                    .AsNoTracking()
                    .Where(t => t.IsActive &&
                    ( t.Edition.DisplayName == TACHYONConsts.CarrierEdtionName || t.Edition.DisplayName == TACHYONConsts.BrokerEditionName))
                    .Select(r => new CarriersForDropDownDto { Id = r.Id, DisplayName = r.companyName, Rate = r.Rate })
                    .ToListAsync();
            }
            else if (await _featureChecker.IsEnabledAsync(AppFeatures.Shipper) &&
                     await _featureChecker.IsEnabledAsync(AppFeatures.SendDirectRequest))
            {
                list = await _tenantCarrierRepository
                    .GetAll()
                    .AsNoTracking()
                    .Where(t => t.CarrierShipper.IsActive)
                    .Select(r => new CarriersForDropDownDto
                    {
                        Id = r.CarrierTenantId,
                        DisplayName = r.CarrierShipper.companyName,
                        Rate = r.CarrierShipper.Rate
                    }).ToListAsync();
            }
            else
            {
                return null;
            }

            return list;
        }


        public async Task<ShippingRequestDirectRequest> Create(CreateShippingRequestDirectRequestInput input)
        {
            var entity = ObjectMapper.Map<ShippingRequestDirectRequest>(input);
            var id = await _shippingRequestDirectRequestRepository.InsertAndGetIdAsync(entity);
            // todo =======>   find workaround and customize dto to complete carrier price package process
            if (input.PricePackageId.HasValue)
            {
                using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MayHaveTenant, AbpDataFilters.MustHaveTenant))
                {
                    var lookupDto = await (from request in _shippingRequestRepository.GetAll()
                                           from pricePackage in _pricePackageRepository.GetAll()
                                           from tenant in _tenantRepository.GetAll()
                                           where request.Id == input.ShippingRequestId && pricePackage.Id == input.PricePackageId &&
                                                 tenant.Id == AbpSession.TenantId
                                           select new
                                           {
                                               RequestReference = request.ReferenceNumber,
                                               PackageReference = pricePackage.PricePackageReference,
                                               CurrentTenantName = tenant.Name
                                           }).SingleAsync();

                    await _appNotifier.NotfiyCarrierWhenReceiveBidPricePackage(input.CarrierTenantId,
                        lookupDto.CurrentTenantName, lookupDto.PackageReference, id, lookupDto.RequestReference);
                }
            }
            else
            {
                string currentTenantName;
                using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MayHaveTenant, AbpDataFilters.MustHaveTenant))
                {
                    currentTenantName = await _tenantRepository.GetAll().Where(x => x.Id == AbpSession.TenantId)
                        .Select(x => x.Name).SingleAsync();
                }

                await _appNotifier.SendDriectRequest(currentTenantName, input.CarrierTenantId, id);
            }

            return entity;
        }
    }
}