using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TACHYON.Configuration;
using TACHYON.PricePackages.Dto.TmsPricePackages;
using TACHYON.PricePackages.TmsPricePackageOffers;
using TACHYON.Shipping.ShippingRequests;

namespace TACHYON.PricePackages.TmsPricePackages
{
    public class TmsPricePackageManager : TACHYONDomainServiceBase, ITmsPricePackageManager
    {
        private readonly IConfigurationProvider _configurationProvider;
        private readonly IRepository<TmsPricePackage> _tmsPricePackageRepository;
        private readonly IRepository<NormalPricePackage> _normalPricePackage;
        private readonly IRepository<ShippingRequest, long> _shippingRequestRepository;
        private readonly IRepository<TmsPricePackageOffer, long> _tmsPricePackageOfferRepository;

        public TmsPricePackageManager(
            IRepository<TmsPricePackage> tmsPricePackageRepository,
            IRepository<ShippingRequest, long> shippingRequestRepository,
            IRepository<NormalPricePackage> normalPricePackage,
            IRepository<TmsPricePackageOffer, long> tmsPricePackageOfferRepository)
        {
            _tmsPricePackageRepository = tmsPricePackageRepository;
            _shippingRequestRepository = shippingRequestRepository;
            _normalPricePackage = normalPricePackage;
            _tmsPricePackageOfferRepository = tmsPricePackageOfferRepository;
            _configurationProvider = IocManager.Instance.Resolve<IMapper>()?.ConfigurationProvider;
        }

        
        public async Task<List<PricePackageSelectItemDto>> GetPricePackagesForCarrierAppendix(int carrierId, int? appendixId = null)
        {
            DisableTenancyFilters();
            
            var tmsPricePackages = await _tmsPricePackageRepository.GetAll().AsNoTracking()
                .Where(x => x.DestinationTenantId == carrierId)
                .WhereIf(appendixId.HasValue,
                    x => (!x.AppendixId.HasValue && !x.ProposalId.HasValue) || x.AppendixId == appendixId)
                .ProjectTo<PricePackageSelectItemDto>(_configurationProvider).ToListAsync();
            
            var normalPricePackages = await _normalPricePackage.GetAll().AsNoTracking()
                .Where(x => x.TenantId == carrierId)
                .ProjectTo<PricePackageSelectItemDto>(_configurationProvider).ToListAsync();
            
            
            var result = new List<PricePackageSelectItemDto>();
            result.AddRange(tmsPricePackages);
            result.AddRange(normalPricePackages);
            
            return result;
        }

        private IQueryable<NormalPricePackage> GetMatchedNormalPricePackage(long shippingRequestId, long directRequestId)
        {
            return (from shippingRequest in _shippingRequestRepository.GetAll()
                where shippingRequest.Id == shippingRequestId
                from pricePackageOffer in _tmsPricePackageOfferRepository.GetAll()
                where pricePackageOffer.DirectRequestId == directRequestId
                from normalPricePackage in _normalPricePackage.GetAll().AsNoTracking()
                where normalPricePackage.Id == pricePackageOffer.NormalPricePackageId &&
                    normalPricePackage.TrucksTypeId == shippingRequest.TrucksTypeId &&
                      normalPricePackage.OriginCityId == shippingRequest.OriginCityId
                      && shippingRequest.ShippingRequestDestinationCities.Any(c =>
                          c.CityId == normalPricePackage.DestinationCityId) 
                select normalPricePackage);
        }

        private IQueryable<TmsPricePackage> GetMatchedTmsPricePackage(long shippingRequestId, long directRequestId)
        {
            return (from shippingRequest in _shippingRequestRepository.GetAll()
                where shippingRequest.Id == shippingRequestId
                from pricePackageOffer in _tmsPricePackageOfferRepository.GetAll()
                where pricePackageOffer.DirectRequestId == directRequestId
                from tmsPricePackage in _tmsPricePackageRepository.GetAll().AsNoTracking()
                where (tmsPricePackage.ProposalId.HasValue || tmsPricePackage.AppendixId.HasValue) &&
                      (!tmsPricePackage.ProposalId.HasValue ||
                       (tmsPricePackage.Proposal.Status == ProposalStatus.Approved &&
                        tmsPricePackage.Proposal.AppendixId.HasValue &&
                        tmsPricePackage.Proposal.Appendix.Status == AppendixStatus.Confirmed &&
                        tmsPricePackage.Proposal.Appendix.IsActive &&
                        tmsPricePackage.Proposal.ShipperId == shippingRequest.TenantId)) &&
                      (!tmsPricePackage.AppendixId.HasValue ||
                       (tmsPricePackage.Appendix.Status == AppendixStatus.Confirmed &&
                        tmsPricePackage.Appendix.IsActive &&
                        !tmsPricePackage.ProposalId.HasValue)) &&
                      tmsPricePackage.Id == pricePackageOffer.TmsPricePackageId &&
                      tmsPricePackage.Type == PricePackageType.PerTrip &&
                      tmsPricePackage.TrucksTypeId == shippingRequest.TrucksTypeId &&
                      tmsPricePackage.OriginCityId == shippingRequest.OriginCityId &&
                      tmsPricePackage.RouteType == shippingRequest.RouteTypeId && shippingRequest
                          .ShippingRequestDestinationCities
                          .Any(i => i.CityId == tmsPricePackage.DestinationCityId)
                orderby tmsPricePackage.Id
                select tmsPricePackage);
        }

        public async Task<decimal?> GetItemPriceByMatchedPricePackage(long shippingRequestId, long directRequestId)
        {
            // find item price from matched normal price package repository
            
            decimal matchedPricePackageItemPrice =
                await (from normalPricePackage in GetMatchedNormalPricePackage(shippingRequestId, directRequestId)
                select normalPricePackage.TachyonMSRequestPrice).FirstOrDefaultAsync();

            // if matched normal price package not found
            if (matchedPricePackageItemPrice == default)
            {
                // find item price from matched tms price package repository
                matchedPricePackageItemPrice = await (from tmsPricePackage in GetMatchedTmsPricePackage(shippingRequestId, directRequestId)
                    orderby tmsPricePackage.Id select tmsPricePackage.TotalPrice).FirstOrDefaultAsync();
            }

            return matchedPricePackageItemPrice;
        }

        public async Task<bool> IsHaveMatchedPricePackage(long shippingRequestId, long? directRequestId)
        {
            if (!directRequestId.HasValue) return false;
            
            bool isHaveNormalPricePackage = await GetMatchedNormalPricePackage(shippingRequestId, directRequestId.Value).AnyAsync();

            if (isHaveNormalPricePackage) return true;

            bool isHaveTmsPricePackage = await GetMatchedTmsPricePackage(shippingRequestId, directRequestId.Value).AnyAsync();

            return isHaveTmsPricePackage;
        }
    }
}