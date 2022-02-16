using Abp.Domain.Repositories;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TACHYON.PriceOffers;
using TACHYON.PriceOffers.Dto;
using TACHYON.PricePackages.Dto.NormalPricePackage;
using TACHYON.Shipping.ShippingRequests;
using TACHYON.ShippingRequestVases;
using TACHYON.Vases;

namespace TACHYON.PricePackages
{
    public class NormalPricePackageManager : TACHYONDomainServiceBase
    {
        private readonly IRepository<NormalPricePackage> _normalPricePackageRepository;
        private readonly IRepository<ShippingRequest, long> _shippingRequestRepository;
        private readonly IRepository<ShippingRequestVas, long> _shippingRequestVasRepository;
        private readonly IRepository<VasPrice> _priceVasRepository;
        private readonly PriceOfferManager _priceOfferManager;

        public NormalPricePackageManager(
            IRepository<NormalPricePackage> normalPricePackageRepository,
            IRepository<ShippingRequest, long> shippingRequestRepository,
            IRepository<ShippingRequestVas, long> shippingRequestVasRepository,
            IRepository<VasPrice> priceVasRepository,
            PriceOfferManager priceOfferManager)
        {
            _normalPricePackageRepository = normalPricePackageRepository;
            _shippingRequestRepository = shippingRequestRepository;
            _shippingRequestVasRepository = shippingRequestVasRepository;
            _priceVasRepository = priceVasRepository;
            _priceOfferManager = priceOfferManager;
        }



        public async Task<BidNormalPricePackageDto> GetBidNormalPricePackageDto(int pricePackageId, long shippingRequestId)
        {
            DisableTenancyFilters();
            //get  price package
            var pricePackage = await GetNormalPricePackage(pricePackageId);
            //get shipping request 
            var shippingRequest = await GetShippingRequest(shippingRequestId);
            var price = GetPriceByShippingRequestType(pricePackage, shippingRequest);

            var tripPrice = CalculateNormalPricePreCommissionAndVat(price, shippingRequest.NumberOfDrops, pricePackage.IsMultiDrop, pricePackage.PricePerExtraDrop);

            var res = new CreateOrEditPriceOfferInput
            {
                ShippingRequestId = shippingRequestId,
                ItemPrice = tripPrice,
                ItemDetails = await CalculateShippingRequestVases(shippingRequestId, pricePackage.TenantId),
            };

            var dto = ObjectMapper.Map<BidNormalPricePackageDto>(await _priceOfferManager.InitPriceOffer(res));

            ObjectMapper.Map(pricePackage, dto);
            dto.NumberOfDrops = shippingRequest.NumberOfDrops;
            dto.NumberOfTrips = shippingRequest.NumberOfTrips;

            foreach (var item in dto.Items)
            {
                item.ItemName = shippingRequest.ShippingRequestVases.FirstOrDefault(x => x.VasId == item.SourceId)?.VasFk?.Name ?? "";
            }

            return dto;
        }


        #region Helpers   
        private async Task<NormalPricePackage> GetNormalPricePackage(int pricePackageId)
        {
            var pricePackage = await _normalPricePackageRepository
                .GetAllIncluding(x => x.OriginCityFK, b => b.DestinationCityFK, t => t.TrucksTypeFk)
                .AsNoTracking().FirstOrDefaultAsync(x => x.Id == pricePackageId);
            if (pricePackage == null) throw new UserFriendlyException();
            return pricePackage;
        }
        private async Task<ShippingRequest> GetShippingRequest(long shippingRequestId)
        {
            var shippingRequest = await _shippingRequestRepository.GetAll()
                .Include(x => x.Tenant)
                .Include(x => x.ShippingRequestVases)
                .ThenInclude(x => x.VasFk)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == shippingRequestId);
            if (shippingRequest == null) throw new UserFriendlyException();
            return shippingRequest;
        }
        private async Task<List<PriceOfferDetailDto>> CalculateShippingRequestVases(long shippingRequestId, int carrierTenantId)
        {
            var shippingRequestVases = await _shippingRequestVasRepository.GetAll()
                .Include(x => x.VasFk)
                .Where(v => v.ShippingRequestId == shippingRequestId).AsNoTracking().ToListAsync();

            var vasesIds = shippingRequestVases.Select(x => x.VasId).ToList();
            var pricedVases = await _priceVasRepository.GetAll()

                .Where(v => v.TenantId == carrierTenantId
                && vasesIds.Contains(v.VasId)).AsNoTracking().ToListAsync();

            var res = new List<PriceOfferDetailDto>();

            foreach (var vas in shippingRequestVases)
            {
                var pricedVas = pricedVases.FirstOrDefault(x => x.VasId == vas.VasId);
                var vasDto = new PriceOfferDetailDto
                {
                    ItemId = vas.Id,
                    Price = Convert.ToInt32(pricedVas.Price.HasValue ? (decimal)pricedVas.Price : 0),
                };
                res.Add(vasDto);
            }
            return res;
        }
        private decimal GetPriceByShippingRequestType(NormalPricePackage pricePackage, ShippingRequest shippingRequest)
        {
            if (shippingRequest.IsDirectRequest) return pricePackage.DirectRequestPrice;
            else if (shippingRequest.IsBid) return pricePackage.MarcketPlaceRequestPrice;
            else return pricePackage.TachyonMSRequestPrice;
        }
        private decimal CalculateNormalPricePreCommissionAndVat(decimal price, int numberOfDrops, bool isMultiDrops, decimal? pricePerDrop)
        {
            var totalPrice = price;
            if (isMultiDrops && numberOfDrops > 1 && pricePerDrop.HasValue) return totalPrice + (numberOfDrops - 1) * pricePerDrop.Value;
            else return totalPrice;
        }
        #endregion
    }
}