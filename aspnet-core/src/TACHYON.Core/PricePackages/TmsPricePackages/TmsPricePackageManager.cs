using Abp.Application.Features;
using Abp.Domain.Repositories;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using TACHYON.Features;
using TACHYON.Shipping.ShippingRequests;

namespace TACHYON.PricePackages.TmsPricePackages
{
    public class TmsPricePackageManager : TACHYONDomainServiceBase, ITmsPricePackageManager
    {
        private readonly IRepository<ShippingRequest, long> _shippingRequestRepository;
        private readonly IRepository<TmsPricePackage> _tmsPricePackage;
        private readonly FeatureChecker _featureChecker;

        public TmsPricePackageManager(
            IRepository<TmsPricePackage> tmsPricePackage,
            IRepository<ShippingRequest, long> shippingRequestRepository,
            FeatureChecker featureChecker)
        {
            _tmsPricePackage = tmsPricePackage;
            _shippingRequestRepository = shippingRequestRepository;
            _featureChecker = featureChecker;
        }

        public async Task<TmsPricePackage> GetMatchingPricePackage(long shippingRequestId)
        {
            if (await _featureChecker.IsEnabledAsync(AppFeatures.TachyonDealer))
                DisableTenancyFilters();
            
            var matchedPricePackage = await (from shippingRequest in _shippingRequestRepository.GetAll()
                where shippingRequest.Id == shippingRequestId
                from tmsPricePackage in _tmsPricePackage.GetAll()
                where tmsPricePackage.Proposal.Status == ProposalStatus.Approved &&
                      tmsPricePackage.Proposal.ShipperId == shippingRequest.TenantId &&
                      tmsPricePackage.Type == PricePackageType.PerTrip &&
                      tmsPricePackage.TrucksTypeId == shippingRequest.TrucksTypeId &&
                      tmsPricePackage.OriginCityId == shippingRequest.OriginCityId &&
                      tmsPricePackage.RouteType == shippingRequest.RouteTypeId
                      && shippingRequest.ShippingRequestDestinationCities.Any(i=> i.CityId == tmsPricePackage.DestinationCityId)
                select tmsPricePackage).FirstOrDefaultAsync();

            return matchedPricePackage;
        }
    }
}