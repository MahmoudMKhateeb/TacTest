using Abp;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TACHYON.Notifications;
using TACHYON.PriceOffers.Dto;
using TACHYON.PricePackages.Dto;
using TACHYON.PricePackages.PricePackageOffers;
using TACHYON.Shipping.ShippingRequestBids;
using TACHYON.Shipping.ShippingRequests;

namespace TACHYON.PricePackages
{
    public class PricePackageManager : TACHYONDomainServiceBase, IPricePackageManager
    {
        private readonly IConfigurationProvider _configurationProvider;
        private readonly IRepository<PricePackage,long> _pricePackageRepository;
        private readonly IRepository<ShippingRequest, long> _shippingRequestRepository;
        private readonly IRepository<PricePackageOffer, long> _pricePackageOfferRepository;
        private readonly IBidDomainService _bidDomainService;
        private readonly IAppNotifier _appNotifier;

        public PricePackageManager(
            IRepository<PricePackage,long> pricePackageRepository,
            IRepository<ShippingRequest, long> shippingRequestRepository,
            IRepository<PricePackageOffer, long> pricePackageOfferRepository,
            IBidDomainService bidDomainService,
            IAppNotifier appNotifier)
        {
            _pricePackageRepository = pricePackageRepository;
            _shippingRequestRepository = shippingRequestRepository;
            _pricePackageOfferRepository = pricePackageOfferRepository;
            _bidDomainService = bidDomainService;
            _appNotifier = appNotifier;
            _configurationProvider = IocManager.Instance.Resolve<IMapper>()?.ConfigurationProvider;
        }

        
        public async Task<List<PricePackageSelectItemDto>> GetPricePackagesForCarrierAppendix(int carrierId, int? appendixId = null)
        {
            DisableTenancyFilters();
            
            return await _pricePackageRepository.GetAll().AsNoTracking()
                .Where(x => x.TenantId == carrierId)
                .WhereIf(appendixId.HasValue,
                    x => (!x.AppendixId.HasValue && !x.ProposalId.HasValue) || x.AppendixId == appendixId)
                .ProjectTo<PricePackageSelectItemDto>(_configurationProvider).ToListAsync();
        }


        private IQueryable<PricePackage> GetMatchedPricePackage(long shippingRequestId, long directRequestId)
        {
            return (from shippingRequest in _shippingRequestRepository.GetAll()
                where shippingRequest.Id == shippingRequestId
                from pricePackageOffer in _pricePackageOfferRepository.GetAll()
                where pricePackageOffer.DirectRequestId == directRequestId
                from pricePackage in _pricePackageRepository.GetAll().AsNoTracking()
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
                      pricePackage.Id == pricePackageOffer.PricePackageId &&
                      pricePackage.Type == PricePackageType.PerTrip &&
                      pricePackage.TruckTypeId == shippingRequest.TrucksTypeId &&
                      pricePackage.OriginCityId == shippingRequest.OriginCityId &&
                      pricePackage.RouteType == shippingRequest.RouteTypeId && shippingRequest
                          .ShippingRequestDestinationCities
                          .Any(i => i.CityId == pricePackage.DestinationCityId)
                orderby pricePackage.Id
                select pricePackage);
        }

        public async Task<decimal?> GetItemPriceByMatchedPricePackage(long shippingRequestId, long directRequestId)
        {
            // find item price from matched price package
            return await (from pricePackage in GetMatchedPricePackage(shippingRequestId, directRequestId)
                orderby pricePackage.Id
                select pricePackage.TotalPrice).FirstOrDefaultAsync();
        }

        public async Task<bool> IsHaveMatchedPricePackage(long shippingRequestId, long? directRequestId)
        {
            if (!directRequestId.HasValue) return false;
            return await GetMatchedPricePackage(shippingRequestId, directRequestId.Value).AnyAsync();
        }
        
        public string GeneratePricePackageReferenceNumber(PricePackage pricePackage)
        {
            string routType = pricePackage.RouteType is null ? "DR" : pricePackage.RouteType == ShippingRequestRouteType.MultipleDrops ? "MUL" : "SDR";
            string formatDate = pricePackage.CreationTime.ToString("ddMMyy");
            long referenceId = pricePackage.Id + 1_000L;
            // NCP ===> Normal Carrier Package
            string usageType = pricePackage.UsageType == PricePackageUsageType.AsTachyonManageService ? "TMS" : "NCP";
            const string referenceNumber = "{0}-{1}-{2}-{3}";
            return string.Format(referenceNumber, formatDate, routType, referenceId,usageType);
        }
        
        public async Task SendNotificationToCarriersWithTheSameTrucks(ShippingRequest shippingRequest)
         {
             if (shippingRequest.BidStatus == ShippingRequestBidStatus.OnGoing)
             {
                 if (!shippingRequest.TrucksTypeId.HasValue) return;
                 
                 UserIdentifier[] users =
                     await _bidDomainService.GetCarriersByTruckTypeArrayAsync(shippingRequest.TrucksTypeId.Value);
                 await _appNotifier.ShippingRequestAsBidWithSameTruckAsync(users, shippingRequest.Id);

                 if(shippingRequest.ShippingRequestDestinationCities?.Count == 1)
                 {
                     var carriers = await GetCarriersMatchingPricePackages(shippingRequest.TrucksTypeId, shippingRequest.OriginCityId, shippingRequest.ShippingRequestDestinationCities.First().CityId);
                     await _appNotifier.ShippingRequestAsBidWithMatchingPricePackage(carriers, shippingRequest.ReferenceNumber, shippingRequest.Id);
                 }
             }
         }
        

        private async Task<List<CarrierPricePackageDto>> GetCarriersMatchingPricePackages(long? truckType,
            int? originCityId, int? destinationCityId)
        {
            DisableTenancyFilters();
            var query = MatchingPricePackageQuery(truckType, originCityId, destinationCityId);
            return await query.Select(c => new CarrierPricePackageDto
            {
                CarrierTenantId = c.TenantId , PricePackageReference = c.PricePackageReference
            }).Distinct().ToListAsync();
        }

        private IQueryable<PricePackage> MatchingPricePackageQuery(long? truckType, int? originCityId,
            int? destinationCityId, int? carrierId = null)
        {
            return _pricePackageRepository.GetAll().Where(x=> x.UsageType == PricePackageUsageType.AsCarrier)
                .WhereIf(carrierId.HasValue, c => c.TenantId == carrierId.Value)
                .Where(x => x.TruckTypeId == truckType && x.OriginCityId == originCityId &&
                            x.DestinationCityId == destinationCityId);
        }
        
        public async Task<long?> GetMatchingPricePackageId(long? truckType, int? originCityId, int? destinationCityId, int? carrierId = null)
         {
             var query = MatchingPricePackageQuery(truckType, originCityId, destinationCityId, carrierId);
             return await query.Select(x => x.Id).FirstOrDefaultAsync();
         }

    }
}