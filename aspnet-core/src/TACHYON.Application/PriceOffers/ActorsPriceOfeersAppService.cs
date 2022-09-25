using Abp.AutoMapper;
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
        private readonly IRepository<ShippingRequest, long> _shippingRequestRepository;

        public ActorsPriceOffersAppService(IRepository<ActorShipperPrice> actorShipperPriceRepository, IRepository<ShippingRequest, long> shippingRequestRepository)
        {
            this._actorShipperPriceRepository = actorShipperPriceRepository;
            this._shippingRequestRepository = shippingRequestRepository;
        }




        public async Task<CreateOrEditActorShipperPriceInput> GetCreateOrEditActorShipperPriceInputForCreateOrEdit(long shippingRequestId)
        {
            DisableTenancyFilters();
            var vasList = (await _shippingRequestRepository
                    .GetAll()
                    .Include(x => x.ShippingRequestVases)
                    .ThenInclude(v => v.VasFk)
                    .FirstAsync(x => x.Id == shippingRequestId))
                .ShippingRequestVases;
               

            var input = new CreateOrEditActorShipperPriceInput();
            input.VasActorShipperPriceDtoPrices = new List<ActorShipperPriceDto>();

            foreach (var vas in vasList)
            {
                input.VasActorShipperPriceDtoPrices.Add(new ActorShipperPriceDto()
                {
                    VasDisplayName = vas.VasFk.Name,
                    VasId = vas.VasFk.Id
                });
            }

            return input;



        }



        public async Task<CreateOrEditActorCarrierPriceInput> GetCreateOrEditActorCarrierPriceInputForCreateOrEdit(long shippingRequestId)
        {
            DisableTenancyFilters();
            var vasList = (await _shippingRequestRepository
                    .GetAll()
                    .Include(x => x.ShippingRequestVases)
                    .ThenInclude(v => v.VasFk)
                    .FirstAsync(x => x.Id == shippingRequestId))
                .ShippingRequestVases;


            var input = new CreateOrEditActorCarrierPriceInput();
            input.VasActorCarrierPriceDtoPrices = new List<ActorCarrierPriceDto>();

            foreach (var vas in vasList)
            {
                input.VasActorCarrierPriceDtoPrices.Add(new ActorCarrierPriceDto()
                {
                    VasDisplayName = vas.VasFk.Name,
                    VasId = vas.VasFk.Id
                });
            }

            return input;



        }

        public async Task CreateOrEditActorShipperPrice(CreateOrEditActorShipperPriceInput input)
        {
            input.ActorShipperPriceDto.TaxVat = 15;
            input.ActorShipperPriceDto.VatAmountWithCommission = input.ActorShipperPriceDto.SubTotalAmountWithCommission * 0.15m;
            input.ActorShipperPriceDto.TotalAmountWithCommission = input.ActorShipperPriceDto.SubTotalAmountWithCommission + input.ActorShipperPriceDto.VatAmountWithCommission;



            var shippingRequest = await _shippingRequestRepository.GetAll()
            .Include(x => x.ShippingRequestTrips)
                .ThenInclude(t=> t.ShippingRequestTripVases)
            .ThenInclude(v=> v.ShippingRequestVasFk)
                .SingleAsync(x => x.Id == input.ShippingRequestId);



            foreach (var shippingRequestTrip in shippingRequest.ShippingRequestTrips)
            {
                shippingRequestTrip.ActorShipperPriceFk = ObjectMapper.Map<ActorShipperPrice>(input.ActorShipperPriceDto);
                if (shippingRequestTrip.ShippingRequestTripVases == null)
                {
                    continue;
                }

                foreach (ShippingRequestTripVas shippingRequestTripVase in shippingRequestTrip.ShippingRequestTripVases)
                {
                    var vasActorShipperPriceDto = input.VasActorShipperPriceDtoPrices.FirstOrDefault(x => x.VasId == shippingRequestTripVase.ShippingRequestVasFk.VasId);

                    if (vasActorShipperPriceDto != null)
                    {
                        shippingRequestTripVase.ActorShipperPriceFk = ObjectMapper.Map<ActorShipperPrice>(vasActorShipperPriceDto);
                    }
                }
            }

    


        }


        public async Task CreateOrEditActorCarrierPrice(CreateOrEditActorCarrierPriceInput input)
        {

            var shippingRequest = await _shippingRequestRepository.GetAll()
          .Include(x => x.ShippingRequestTrips)
              .ThenInclude(t => t.ShippingRequestTripVases)
          .ThenInclude(v => v.ShippingRequestVasFk)
              .SingleAsync(x => x.Id == input.ShippingRequestId);

            foreach (var shippingRequestTrip in shippingRequest.ShippingRequestTrips)
            {
                shippingRequestTrip.ActorCarrierPriceFk = ObjectMapper.Map<ActorCarrierPrice>(input.ActorCarrierPriceDto);

                if (shippingRequestTrip.ShippingRequestTripVases == null)
                {
                    continue;
                }

                foreach (ShippingRequestTripVas shippingRequestTripVase in shippingRequestTrip.ShippingRequestTripVases)
                {
                    var vasActorCarrierPriceDto = input.VasActorCarrierPriceDtoPrices.FirstOrDefault(x => x.VasId == shippingRequestTripVase.ShippingRequestVasFk.VasId);

                    if (vasActorCarrierPriceDto != null)
                    {
                        shippingRequestTripVase.ActorCarrierPriceFk = ObjectMapper.Map<ActorCarrierPrice>(vasActorCarrierPriceDto);
                    }
                }
            }

        }

    }


}
