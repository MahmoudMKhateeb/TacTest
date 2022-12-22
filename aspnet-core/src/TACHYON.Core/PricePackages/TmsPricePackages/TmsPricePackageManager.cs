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
using TACHYON.Shipping.ShippingRequests;

namespace TACHYON.PricePackages.TmsPricePackages
{
    public class TmsPricePackageManager : TACHYONDomainServiceBase, ITmsPricePackageManager
    {
        private readonly IConfigurationProvider _configurationProvider;
        private readonly IRepository<TmsPricePackage> _tmsPricePackageRepository;
        private readonly IRepository<NormalPricePackage> _normalPricePackage;
        private readonly IRepository<ShippingRequest, long> _shippingRequestRepository;

        public TmsPricePackageManager(
            IRepository<TmsPricePackage> tmsPricePackageRepository,
            IRepository<ShippingRequest, long> shippingRequestRepository,
            IRepository<NormalPricePackage> normalPricePackage)
        {
            _tmsPricePackageRepository = tmsPricePackageRepository;
            _shippingRequestRepository = shippingRequestRepository;
            _normalPricePackage = normalPricePackage;
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

        private IQueryable<NormalPricePackage> GetMatchedNormalPricePackage(long shippingRequestId, int carrierId)
        {
            return (from shippingRequest in _shippingRequestRepository.GetAll()
                where shippingRequest.Id == shippingRequestId
                from normalPricePackage in _normalPricePackage.GetAll().AsNoTracking()
                where normalPricePackage.TenantId == carrierId &&
                    normalPricePackage.TrucksTypeId == shippingRequest.TrucksTypeId &&
                      normalPricePackage.OriginCityId == shippingRequest.OriginCityId
                      && shippingRequest.ShippingRequestDestinationCities.Any(c =>
                          c.CityId == normalPricePackage.DestinationCityId) 
                select normalPricePackage);
        }

        private IQueryable<TmsPricePackage> GetMatchedTmsPricePackage(long shippingRequestId, int carrierId)
        {
            return (from shippingRequest in _shippingRequestRepository.GetAll()
                where shippingRequest.Id == shippingRequestId
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
                      tmsPricePackage.DestinationTenantId == carrierId &&
                      tmsPricePackage.Type == PricePackageType.PerTrip &&
                      tmsPricePackage.TrucksTypeId == shippingRequest.TrucksTypeId &&
                      tmsPricePackage.OriginCityId == shippingRequest.OriginCityId &&
                      tmsPricePackage.RouteType == shippingRequest.RouteTypeId && shippingRequest
                          .ShippingRequestDestinationCities
                          .Any(i => i.CityId == tmsPricePackage.DestinationCityId)
                orderby tmsPricePackage.Id
                select tmsPricePackage);
        }

        public async Task<decimal?> GetItemPriceByMatchedPricePackage(long shippingRequestId,decimal quantity, int carrierId)
        {
            // find item price from matched normal price package repository
            
            decimal matchedPricePackageItemPrice =
                await (from normalPricePackage in GetMatchedNormalPricePackage(shippingRequestId, carrierId)
                select normalPricePackage.TachyonMSRequestPrice).FirstOrDefaultAsync();

            // if matched normal price package not found
            if (matchedPricePackageItemPrice == default)
            {
                // find item price from matched tms price package repository
                matchedPricePackageItemPrice = await (from tmsPricePackage in GetMatchedTmsPricePackage(shippingRequestId, carrierId)
                    orderby tmsPricePackage.Id select tmsPricePackage.TotalPrice).FirstOrDefaultAsync();
            }

            return matchedPricePackageItemPrice;
        }

        public async Task<bool> IsHaveMatchedPricePackage(long shippingRequestId, int carrierId)
        {
            bool isHaveNormalPricePackage = await GetMatchedNormalPricePackage(shippingRequestId, carrierId).AnyAsync();

            if (isHaveNormalPricePackage) return true;

            bool isHaveTmsPricePackage = await GetMatchedTmsPricePackage(shippingRequestId, carrierId).AnyAsync();

            return isHaveTmsPricePackage;
        }
    }
}