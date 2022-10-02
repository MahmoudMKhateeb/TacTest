using Abp.AutoMapper;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TACHYON.Actors.Dtos;
using TACHYON.Extension;
using TACHYON.Shipping.ShippingRequests;
using TACHYON.Shipping.ShippingRequestTrips;
using TACHYON.ShippingRequestTripVases;

namespace TACHYON.PriceOffers
{
    [AutoMapTo(typeof(ActorShipperPrice))]
    [AutoMapFrom(typeof(ActorShipperPrice))]
    public class ActorShipperPriceDto
    {
        /// <summary>
        /// to pass Vas display name to front 
        /// </summary>
        public string VasDisplayName { get; set; }

        public long VasId{ get; set; }


        public decimal? TotalAmountWithCommission { get; set; }
        public decimal? SubTotalAmountWithCommission { get; set; }
        public decimal? VatAmountWithCommission { get; set; }
        public decimal? TaxVat { get; set; }
    }



    public class CreateOrEditActorShipperPriceInput
    {


        public ActorShipperPriceDto ActorShipperPriceDto { get; set; }

        public List<ActorShipperPriceDto> VasActorShipperPriceDtoPrices { get; set; }
        public long ShippingRequestId { get; set; }

        public int DueDates { get; set; }
    }




    [AutoMapTo(typeof(ActorCarrierPrice))]
    [AutoMapFrom(typeof(ActorCarrierPrice))]
    public class ActorCarrierPriceDto
    {

        /// <summary>
        /// to pass Vas display name to front 
        /// </summary>
        public string VasDisplayName { get; set; }

        public long VasId { get; set; }

        public decimal? SubTotalAmount { get; set; }
        public decimal? VatAmount { get; set; }
        public decimal? TaxVat { get; set; }

    }



    public class CreateOrEditActorCarrierPriceInput
    {


        public ActorCarrierPriceDto ActorCarrierPriceDto { get; set; }

        public List<ActorCarrierPriceDto> VasActorCarrierPriceDtoPrices { get; set; }
        public long ShippingRequestId { get; set; }

        public int DueDates { get; set; }
    }




    public class ActorsPriceOffersAppService : TACHYONAppServiceBase
    {

        private readonly IRepository<ActorShipperPrice> _actorShipperPriceRepository;
        private readonly IRepository<ActorCarrierPrice> _actorCarrierPriceRepository;
        private readonly IRepository<ShippingRequest, long> _shippingRequestRepository;

        public ActorsPriceOffersAppService(
            IRepository<ActorShipperPrice> actorShipperPriceRepository,
            IRepository<ShippingRequest, long> shippingRequestRepository,
            IRepository<ActorCarrierPrice> actorCarrierPriceRepository)
        {
            this._actorShipperPriceRepository = actorShipperPriceRepository;
            this._shippingRequestRepository = shippingRequestRepository;
            _actorCarrierPriceRepository = actorCarrierPriceRepository;
        }




        public async Task<CreateOrEditActorShipperPriceInput> GetCreateOrEditActorShipperPriceInputForCreateOrEdit(long shippingRequestId)
        {
            DisableTenancyFilters();
            
            var vasList = (
                from srVas in _shippingRequestRepository.GetAll().SelectMany(x=> x.ShippingRequestVases)
                where srVas.ShippingRequestId == shippingRequestId
                from shipperVasPrice in _actorShipperPriceRepository.GetAll()
                    .Where(x => x.ShippingRequestTripVasFk.ShippingRequestVasId == srVas.Id).DefaultIfEmpty()
                select new ActorShipperPriceDto()
                {
                    VasDisplayName = srVas.VasFk.Name,
                    VasId = srVas.VasId,
                    TotalAmountWithCommission = shipperVasPrice != null? shipperVasPrice.TotalAmountWithCommission: default,
                    SubTotalAmountWithCommission = shipperVasPrice != null? shipperVasPrice.SubTotalAmountWithCommission: default,
                    VatAmountWithCommission = shipperVasPrice != null? shipperVasPrice.VatAmountWithCommission: default,
                    TaxVat = shipperVasPrice != null? shipperVasPrice.TaxVat: default
                });

            var input = new CreateOrEditActorShipperPriceInput();
            input.VasActorShipperPriceDtoPrices = await vasList.ToListAsync();
            var tripPrice = await (from shipperTripPrice in _actorShipperPriceRepository.GetAll()
                where shipperTripPrice.ShippingRequestTripFk.ShippingRequestId == shippingRequestId
                select shipperTripPrice).FirstOrDefaultAsync();
            input.ActorShipperPriceDto = ObjectMapper.Map<ActorShipperPriceDto>(tripPrice);

            return input;



        }



        public async Task<CreateOrEditActorCarrierPriceInput> GetCreateOrEditActorCarrierPriceInputForCreateOrEdit(long shippingRequestId)
        {
            DisableTenancyFilters();
            var vasList = (
                from srVas in _shippingRequestRepository.GetAll().SelectMany(x=> x.ShippingRequestVases)
                where srVas.ShippingRequestId == shippingRequestId
                from carrierVasPrice in _actorCarrierPriceRepository.GetAll()
                    .Where(x => x.ShippingRequestTripVasFk.ShippingRequestVasId == srVas.Id).DefaultIfEmpty()
                select new ActorCarrierPriceDto()
                {
                    VasDisplayName = srVas.VasFk.Name,
                    VasId = srVas.VasId,
                    SubTotalAmount = carrierVasPrice != null? carrierVasPrice.SubTotalAmount: default,
                    VatAmount = carrierVasPrice != null? carrierVasPrice.VatAmount: default,
                    TaxVat = carrierVasPrice != null? carrierVasPrice.TaxVat: default
                });

            var input = new CreateOrEditActorCarrierPriceInput
            {
                VasActorCarrierPriceDtoPrices = await vasList.ToListAsync()
            };
            var tripPrice = await (from carrierTripPrice in _actorCarrierPriceRepository.GetAll()
                where carrierTripPrice.ShippingRequestTripFk.ShippingRequestId == shippingRequestId
                select carrierTripPrice).FirstOrDefaultAsync();
            input.ActorCarrierPriceDto = ObjectMapper.Map<ActorCarrierPriceDto>(tripPrice);

            return input;



        }

        public async Task CreateOrEditActorShipperPrice(CreateOrEditActorShipperPriceInput input)
        {
            input.ActorShipperPriceDto.TaxVat = 15;
            input.ActorShipperPriceDto.VatAmountWithCommission = input.ActorShipperPriceDto.SubTotalAmountWithCommission * 0.15m;
            input.ActorShipperPriceDto.TotalAmountWithCommission = input.ActorShipperPriceDto.SubTotalAmountWithCommission + input.ActorShipperPriceDto.VatAmountWithCommission;



            var shippingRequest = await _shippingRequestRepository.GetAll()
            .Include(x => x.ShippingRequestTrips)
                .ThenInclude(t=> t.ShippingRequestTripVases).ThenInclude(v=> v.ActorShipperPriceFk)
            .Include(x => x.ShippingRequestTrips)
                .ThenInclude(t=> t.ShippingRequestTripVases)
            .ThenInclude(v=> v.ShippingRequestVasFk)
            .Include(x => x.ShippingRequestTrips)
            .ThenInclude(t=> t.ActorShipperPriceFk)
                .SingleAsync(x => x.Id == input.ShippingRequestId);



            foreach (var shippingRequestTrip in shippingRequest.ShippingRequestTrips)
            {
                // Start Of Create or Edit Actor Shipper Price 
                if (shippingRequestTrip.ActorShipperPriceFk != null) 
                    ObjectMapper.Map(input.ActorShipperPriceDto, shippingRequestTrip.ActorShipperPriceFk);
                
                shippingRequestTrip.ActorShipperPriceFk ??= ObjectMapper.Map<ActorShipperPrice>(input.ActorShipperPriceDto);
                // End Of Create or Edit Actor Shipper Price 
                
                if (shippingRequestTrip.ShippingRequestTripVases.IsNullOrEmpty())
                {
                    continue;
                }

                foreach (ShippingRequestTripVas shippingRequestTripVase in shippingRequestTrip.ShippingRequestTripVases)
                {
                    var vasActorShipperPriceDto = input.VasActorShipperPriceDtoPrices.FirstOrDefault(x => x.VasId == shippingRequestTripVase.ShippingRequestVasFk.VasId);

                    if (vasActorShipperPriceDto == null) continue;

                    if (shippingRequestTripVase.ActorShipperPriceFk != null)
                        ObjectMapper.Map(vasActorShipperPriceDto, shippingRequestTripVase.ActorShipperPriceFk);

                    shippingRequestTripVase.ActorShipperPriceFk ??=
                        ObjectMapper.Map<ActorShipperPrice>(vasActorShipperPriceDto);
                }
            }

    


        }


        public async Task CreateOrEditActorCarrierPrice(CreateOrEditActorCarrierPriceInput input)
        {
            input.ActorCarrierPriceDto.TaxVat = 15;
            var shippingRequest = await _shippingRequestRepository.GetAll()
          .Include(x => x.ShippingRequestTrips)
              .ThenInclude(t => t.ShippingRequestTripVases)
          .ThenInclude(v => v.ShippingRequestVasFk)
          .Include(x=> x.ShippingRequestTrips)
          .ThenInclude(x=> x.ShippingRequestTripVases).ThenInclude(x=> x.ActorCarrierPriceFk)
          .Include(x=> x.ShippingRequestTrips).ThenInclude(x=> x.ActorCarrierPriceFk)
              .SingleAsync(x => x.Id == input.ShippingRequestId);

            foreach (var shippingRequestTrip in shippingRequest.ShippingRequestTrips)
            {
                if (shippingRequestTrip.ActorCarrierPriceFk != null)
                    ObjectMapper.Map(input.ActorCarrierPriceDto, shippingRequestTrip.ActorCarrierPriceFk);
                
                shippingRequestTrip.ActorCarrierPriceFk ??= ObjectMapper.Map<ActorCarrierPrice>(input.ActorCarrierPriceDto);

                if (shippingRequestTrip.ShippingRequestTripVases == null)
                {
                    continue;
                }

                foreach (ShippingRequestTripVas shippingRequestTripVase in shippingRequestTrip.ShippingRequestTripVases)
                {
                    var vasActorCarrierPriceDto = input.VasActorCarrierPriceDtoPrices.FirstOrDefault(x => x.VasId == shippingRequestTripVase.ShippingRequestVasFk.VasId);

                    if (vasActorCarrierPriceDto == null) continue;

                    if (shippingRequestTripVase.ActorCarrierPriceFk != null)
                        ObjectMapper.Map(vasActorCarrierPriceDto, shippingRequestTripVase.ActorCarrierPriceFk);

                    shippingRequestTripVase.ActorCarrierPriceFk ??=
                        ObjectMapper.Map<ActorCarrierPrice>(vasActorCarrierPriceDto);
                    
                }
            }

        }

    }


}
