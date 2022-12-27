using Abp.Authorization;
using Abp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using TACHYON.Authorization;
using TACHYON.PriceOffers.Dto;
using TACHYON.Shipping.ShippingRequests;
using TACHYON.Shipping.ShippingRequestTrips;
using TACHYON.ShippingRequestVases;

namespace TACHYON.PriceOffers
{
    [AbpAuthorize(AppPermissions.Pages_ActorPrices)]
    public class ActorsPriceOffersAppService : TACHYONAppServiceBase, IActorsPriceOffersAppService
    {

        private readonly IRepository<ActorShipperPrice> _actorShipperPriceRepository;
        private readonly IRepository<ActorCarrierPrice> _actorCarrierPriceRepository;
        private readonly IRepository<ShippingRequest, long> _shippingRequestRepository;
        private readonly IRepository<ShippingRequestVas, long> _shippingRequestVasRepository;

        public ActorsPriceOffersAppService(
            IRepository<ActorShipperPrice> actorShipperPriceRepository,
            IRepository<ShippingRequest, long> shippingRequestRepository,
            IRepository<ActorCarrierPrice> actorCarrierPriceRepository,
            IRepository<ShippingRequestVas, long> shippingRequestVasRepository)
        {
            this._actorShipperPriceRepository = actorShipperPriceRepository;
            this._shippingRequestRepository = shippingRequestRepository;
            _actorCarrierPriceRepository = actorCarrierPriceRepository;
            _shippingRequestVasRepository = shippingRequestVasRepository;
        }



        public async Task<CreateOrEditSrActorShipperPriceInput> GetActorShipperPriceForEdit(long shippingRequestId)
        {
            DisableTenancyFilters();
            
              
            var vasList = (
                from srVas in _shippingRequestRepository.GetAll().SelectMany(x=> x.ShippingRequestVases)
                where srVas.ShippingRequestId == shippingRequestId
                select new CreateOrEditActorShipperPriceDto()
                {
                    VasDisplayName = srVas.VasFk.Name,
                    VasId = srVas.VasId,Id = srVas.ActorShipperPriceId,
                    ShippingRequestVasId = srVas.Id,
                    TotalAmountWithCommission = srVas.ActorShipperPrice.TotalAmountWithCommission,
                    SubTotalAmountWithCommission = srVas.ActorShipperPrice.SubTotalAmountWithCommission,
                    VatAmountWithCommission = srVas.ActorShipperPrice.VatAmountWithCommission,
                    TaxVat = srVas.ActorShipperPrice.TaxVat
                });

            var input = new CreateOrEditSrActorShipperPriceInput
            {
                VasActorShipperPriceDto = await vasList.ToListAsync()
            };
            var tripPrice = await (from shipperTripPrice in _actorShipperPriceRepository.GetAll()
                where shipperTripPrice.ShippingRequestId == shippingRequestId
                select shipperTripPrice).FirstOrDefaultAsync();
            input.ActorShipperPriceDto = ObjectMapper.Map<CreateOrEditActorShipperPriceDto>(tripPrice);

            return input;

        }

        [AbpAuthorize(AppPermissions.Pages_ActorPrices_Shipper)]
        public async Task CreateOrEditActorShipperPrice(CreateOrEditSrActorShipperPriceInput input)
        {
            input.ActorShipperPriceDto.TaxVat = 15;
            input.ActorShipperPriceDto.VatAmountWithCommission = input.ActorShipperPriceDto.SubTotalAmountWithCommission * 0.15m;
            input.ActorShipperPriceDto.TotalAmountWithCommission = input.ActorShipperPriceDto.SubTotalAmountWithCommission + input.ActorShipperPriceDto.VatAmountWithCommission;

            DisableTenancyFilters();
            
            if (input.ActorShipperPriceDto.Id.HasValue)
            {
                await UpdateActorShipperPrice(input);
                return;
            }

            await CreateActorShipperPrice(input);
        }

        protected virtual async Task CreateActorShipperPrice(CreateOrEditSrActorShipperPriceInput input)
        {
            if (!input.ActorShipperPriceDto.ShippingRequestId.HasValue) return;

            var createdShipperPrice = ObjectMapper.Map<ActorShipperPrice>(input.ActorShipperPriceDto);
            
            
            foreach (var vasPrice in input.VasActorShipperPriceDto)
            {
                if (!vasPrice.ShippingRequestVasId.HasValue) return;
                var createdVasPrice = ObjectMapper.Map<ActorShipperPrice>(vasPrice);
                var createdVasPriceId = await _actorShipperPriceRepository.InsertAndGetIdAsync(createdVasPrice);
                _shippingRequestVasRepository.Update(vasPrice.ShippingRequestVasId.Value,
                    x => x.ActorShipperPriceId = createdVasPriceId);
            }
            
            var createdShipperPriceId = await _actorShipperPriceRepository.InsertAndGetIdAsync(createdShipperPrice);
            var srId = input.ActorShipperPriceDto.ShippingRequestId.Value;
            _shippingRequestRepository.Update(srId, x => x.ActorShipperPriceId = createdShipperPriceId);
        }
        
        protected virtual async Task UpdateActorShipperPrice(CreateOrEditSrActorShipperPriceInput input)
        {
            if (!input.ActorShipperPriceDto.Id.HasValue) return;
            var srShipperPrice = await _actorShipperPriceRepository.SingleAsync(x=> x.Id == input.ActorShipperPriceDto.Id.Value);
            ObjectMapper.Map(input.ActorShipperPriceDto, srShipperPrice);

            var vasActorShipperPriceIdList = input.VasActorShipperPriceDto.Select(x => x.Id).ToList();
            var vasShipperPrices = await _actorShipperPriceRepository.GetAll()
                .Where(x => vasActorShipperPriceIdList.Contains(x.Id)).ToArrayAsync();
            
            if (input.VasActorShipperPriceDto == null || input.VasActorShipperPriceDto.Count < 1) return;

            if (input.VasActorShipperPriceDto.Any(x => !x.Id.HasValue))
            {
                var addedVasesPrices = input.VasActorShipperPriceDto.Where(x => !x.Id.HasValue).ToList();
                foreach (var vasPrice in addedVasesPrices)
                {
                    if (!vasPrice.ShippingRequestVasId.HasValue) return;
                    var createdVasPrice = ObjectMapper.Map<ActorShipperPrice>(vasPrice);
                    var createdVasPriceId = await _actorShipperPriceRepository.InsertAndGetIdAsync(createdVasPrice);
                    _shippingRequestVasRepository.Update(vasPrice.ShippingRequestVasId.Value,
                        x => x.ActorShipperPriceId = createdVasPriceId);
                }
            }
            
            foreach (var vasShipperPrice in vasShipperPrices)
            {
                var vasShipperPriceDto = input.VasActorShipperPriceDto.Single(x => x.Id == vasShipperPrice.Id);
                ObjectMapper.Map(vasShipperPriceDto, vasShipperPrice);
            }
            
        }
        
        public async Task<CreateOrEditSrActorCarrierPriceInput> GetActorCarrierPriceForEdit(long shippingRequestId)
        {
            DisableTenancyFilters();
            var vasList = (
                from srVas in _shippingRequestRepository.GetAll().AsNoTracking().SelectMany(x=> x.ShippingRequestVases)
                where srVas.ShippingRequestId == shippingRequestId
                select new CreateOrEditActorCarrierPrice()
                {
                    VasDisplayName = srVas.VasFk.Name,
                    VasId = srVas.VasId,Id = srVas.ActorCarrierPriceId,
                    ShippingRequestVasId = srVas.Id,
                    SubTotalAmount = srVas.ActorCarrierPrice.SubTotalAmount,
                    VatAmount = srVas.ActorCarrierPrice.VatAmount,
                    TaxVat = srVas.ActorCarrierPrice.TaxVat
                });

            var output = new CreateOrEditSrActorCarrierPriceInput
            {
                VasActorCarrierPrices = await vasList.ToListAsync()
            };
            
            var actorCarrierPrice = await _actorCarrierPriceRepository.FirstOrDefaultAsync(x => x.ShippingRequestId == shippingRequestId);
            output.ActorCarrierPrice = ObjectMapper.Map<CreateOrEditActorCarrierPrice>(actorCarrierPrice);

            return output;
        }

        [AbpAuthorize(AppPermissions.Pages_ActorPrices_Carrier)]
        public async Task CreateOrEditActorCarrierPrice(CreateOrEditSrActorCarrierPriceInput input)
        {
            input.ActorCarrierPrice.TaxVat = 15;

            DisableTenancyFilters();
            if (!input.ActorCarrierPrice.Id.HasValue)
            {
                await CreateActorCarrierPrice(input);
                return;
            }

            await UpdateActorCarrierPrice(input);
        }

        protected virtual async Task CreateActorCarrierPrice(CreateOrEditSrActorCarrierPriceInput input)
        {
            if (!input.ActorCarrierPrice.ShippingRequestId.HasValue) return;

            var createdCarrierPrice = ObjectMapper.Map<ActorCarrierPrice>(input.ActorCarrierPrice);
            
            
            foreach (var vasPrice in input.VasActorCarrierPrices)
            {
                if (!vasPrice.ShippingRequestVasId.HasValue) return;
                var createdVasPrice = ObjectMapper.Map<ActorCarrierPrice>(vasPrice);
                var createdVasPriceId = await _actorCarrierPriceRepository.InsertAndGetIdAsync(createdVasPrice);
                _shippingRequestVasRepository.Update(vasPrice.ShippingRequestVasId.Value,
                    x => x.ActorCarrierPriceId = createdVasPriceId);
            }
            
            var createdCarrierPriceId = await _actorCarrierPriceRepository.InsertAndGetIdAsync(createdCarrierPrice);
            var srId = input.ActorCarrierPrice.ShippingRequestId.Value;
            _shippingRequestRepository.Update(srId, x => x.ActorCarrierPriceId = createdCarrierPriceId);
        }
        
        protected virtual async Task UpdateActorCarrierPrice(CreateOrEditSrActorCarrierPriceInput input)
        {
            if (!input.ActorCarrierPrice.Id.HasValue) return;
            var srCarrierPrice = await _actorCarrierPriceRepository.SingleAsync(x=> x.Id == input.ActorCarrierPrice.Id.Value);
            ObjectMapper.Map(input.ActorCarrierPrice, srCarrierPrice);


            var vasActorCarrierPriceIdList = input.VasActorCarrierPrices.Select(x => x.Id).ToList();
            var carrierPrices = await _actorCarrierPriceRepository.GetAll()
                .Where(x => vasActorCarrierPriceIdList.Contains(x.Id)).ToArrayAsync();
            
            if (input.VasActorCarrierPrices == null || input.VasActorCarrierPrices.Count < 1) return; 

            
            foreach (var vasPrice in input.VasActorCarrierPrices.Where(x=> !x.Id.HasValue))
            {
                if (!vasPrice.ShippingRequestVasId.HasValue) return;
                var createdVasPrice = ObjectMapper.Map<ActorCarrierPrice>(vasPrice);
                var createdVasPriceId = await _actorCarrierPriceRepository.InsertAndGetIdAsync(createdVasPrice);
                _shippingRequestVasRepository.Update(vasPrice.ShippingRequestVasId.Value,
                    x => x.ActorCarrierPriceId = createdVasPriceId);
            }

            foreach (var vasCarrierPrice in carrierPrices)
            {
                var vasCarrierPriceDto = input.VasActorCarrierPrices.Single(x => x.Id == vasCarrierPrice.Id);
                ObjectMapper.Map(vasCarrierPriceDto, vasCarrierPrice);
            }
            
        }

    }


}
