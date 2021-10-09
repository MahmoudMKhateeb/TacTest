using Abp.Application.Services;
using Abp.AutoMapper;
using Abp.BackgroundJobs;
using Abp.Configuration;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Extensions;
using Abp.Json;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using TACHYON.BayanIntegration.Jobs;
using TACHYON.BayanIntegration.Modules;
using TACHYON.Configuration;
using TACHYON.Documents;
using TACHYON.Routs.RoutPoints;
using TACHYON.Shipping.ShippingRequests;
using TACHYON.Shipping.ShippingRequestTrips;
using TACHYON.ShippingRequestTripVases;

namespace TACHYON.BayanIntegration
{
    public class BayanIntegrationManager : TACHYONDomainServiceBase
    {



        private readonly IRepository<ShippingRequest, long> _shippingRequestRepository;
        private readonly IRepository<ShippingRequestTrip> _shippingRequestTripTripRepository;
        private readonly IRepository<ShippingRequestTripVas, long> _shippingRequestTripVasRepository;
        private readonly DocumentFilesManager _documentFilesManager;
        private readonly ISettingManager _settingManager;
        private readonly IBackgroundJobManager _backgroundJobManager;

        public BayanIntegrationManager(IRepository<ShippingRequest, long> shippingRequestRepository, IRepository<ShippingRequestTrip> shippingRequestTripTripRepository, DocumentFilesManager documentFilesManager, IRepository<ShippingRequestTripVas, long> shippingRequestTripVasRepository, ISettingManager settingManager, IBackgroundJobManager backgroundJobManager)
        {
            _shippingRequestRepository = shippingRequestRepository;
            _shippingRequestTripTripRepository = shippingRequestTripTripRepository;
            _documentFilesManager = documentFilesManager;
            _shippingRequestTripVasRepository = shippingRequestTripVasRepository;
            _settingManager = settingManager;
            _backgroundJobManager = backgroundJobManager;
        }

        private Task<string> Url => _settingManager.GetSettingValueAsync(AppSettings.BayanIntegration.Url);
        private Task<string> AppId => _settingManager.GetSettingValueAsync(AppSettings.BayanIntegration.AppId);
        private Task<string> AppKey => _settingManager.GetSettingValueAsync(AppSettings.BayanIntegration.AppKey);
        private Task<string> ClientId => _settingManager.GetSettingValueAsync(AppSettings.BayanIntegration.ClientId);


        /// <summary>
        /// This is service shall be utilized by Electronic Freight Forwarders to allows their systems to create Consignment note in Bayan system. 
        /// </summary>
        public async Task CreateConsignmentNote(int id)
        {

            using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MayHaveTenant, AbpDataFilters.MustHaveTenant))
            {
                //try
                //{
                Tuple<RootForCreate, ShippingRequestTrip> tuple = await GetRootForCreate(id);
                string body = ToJsonLowerCaseFirstLetter(tuple.Item1);

                var client = new RestClient(await Url + "consignment-notes");
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                request.AddHeader("app_id", await AppId);
                request.AddHeader("app_key", await AppKey);
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("client_id", await ClientId);
                request.AddParameter("application/json", body, ParameterType.RequestBody);
                IRestResponse response = await client.ExecuteAsync(request);
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    Logger.Error("BayanIntegrationManager.CreateConsignmentNote" + response.Content);
                }
                else
                {
                    Logger.Trace("BayanIntegrationManager.CreateConsignmentNote" + response.Content);
                    tuple.Item2.BayanId = response.Content;
                }

                //}
                //catch (Exception e)
                //{
                //    Logger.Error("BayanIntegrationManager.CreateConsignmentNote" + e.Message);
                //}



            }


        }

        public async Task QueueCreateConsignmentNote(int tripId)
        {
            await _backgroundJobManager.EnqueueAsync<CreateConsignmentNoteJob, int>(tripId);
        }

        /// <summary>
        /// This is service shall be utilized by Electronic Freight Forwarders to allows their systems to  update Consignment note in Bayan system 
        /// </summary>
        public async Task EditConsignmentNote(int tripId)
        {
            using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MayHaveTenant, AbpDataFilters.MustHaveTenant))
            {
                try
                {
                    Tuple<RootForEdit, ShippingRequestTrip> tuple = await GetRootForEdit(tripId);
                    string body = ToJsonLowerCaseFirstLetter(tuple.Item1);

                    var client = new RestClient(await Url + "consignment-notes");
                    client.Timeout = -1;
                    var request = new RestRequest(Method.POST);
                    request.AddHeader("app_id", await AppId);
                    request.AddHeader("app_key", await AppKey);
                    request.AddHeader("Content-Type", "application/json");
                    request.AddHeader("client_id", await ClientId);
                    request.AddParameter("application/json", body, ParameterType.RequestBody);
                    IRestResponse response = await client.ExecuteAsync(request);
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        Logger.Error("BayanIntegrationManager.EditConsignmentNote" + response);
                    }
                    else
                    {
                        Logger.Trace("BayanIntegrationManager.EditConsignmentNote" + response);

                    }
                }
                catch (Exception e)
                {
                    Logger.Error("BayanIntegrationManager.EditConsignmentNote" + e.Message);
                }



            }
        }

        public async Task QueueEditConsignmentNote(int tripId)
        {
            await _backgroundJobManager.EnqueueAsync<EditConsignmentNoteJob, int>(tripId);
        }

        /// <summary>
        /// This is service shall be utilized by Electronic Freight Forwarders to allows their systems to  close Consignment note in Bayan system 
        /// </summary>
        public void CloseConsignmentNote()
        {

        }

        private async Task<Tuple<RootForCreate, ShippingRequestTrip>> GetRootForCreate(int id)
        {

            ShippingRequestTrip trip = await _shippingRequestTripTripRepository.GetAsync(id);



            var driverIdentityNumber = (await _documentFilesManager.GetDriverIqamaActiveDocumentAsync(trip.AssignedDriverUserId.Value))?.Number;

            var vehicleSequenceNumber = (await _documentFilesManager.GetTruckIstimaraActiveDocumentAsync(trip.AssignedTruckId.Value))?.Number;


            var extraCharges = (await _shippingRequestTripVasRepository.GetAll()
                .Where(x => x.ShippingRequestTripId == trip.Id)
                .SumAsync(x => x.TotalAmountWithCommission))
                .ToString();


            RootForCreate root = await _shippingRequestTripTripRepository.GetAll()
                .Where(x => x.Id == id)
                .Select(x => new RootForCreate()
                {
                    Sender = new Sender()
                    {
                        Address = x.ShippingRequestFk.Tenant.Address,
                        Comment = x.Note,
                        Name = x.ShippingRequestFk.Tenant.companyName,
                        //todo: must not be blank
                        //todo: must match \"\\+9665\\d{8}\
                        Phone = "+" + x.ShippingRequestFk.Tenant.MobileNo
                    },
                    Recipient = new Recipient()
                    {
                        Address = x.ShippingRequestFk.CarrierTenantFk.Address,
                        //todo: must not be blank 
                        //todo: must match \"\\+9665\\d{8}\
                        Phone = "+" + x.ShippingRequestFk.CarrierTenantFk.MobileNo,
                        Name = x.ShippingRequestFk.CarrierTenantFk.companyName,
                    },
                    PickUpLocation = new PickUpLocation()
                    {
                        CityName = x.DestinationFacilityFk.CityFk.DisplayName,
                        RegionName = x.DestinationFacilityFk.CityFk.CountyFk.DisplayName
                    },
                    DropOffLocation = new DropOffLocation()
                    {
                        CityName = x.OriginFacilityFk.CityFk.DisplayName,
                        RegionName = x.OriginFacilityFk.CityFk.CountyFk.DisplayName
                    },
                    Items = x.RoutPoints
                        .SelectMany(r => r.GoodsDetails)
                        //? no goodsType ?
                        .Select(g => new Item()
                        {
                            DangerousCode = g.DangerousGoodsCode,
                            DangerousGoodTypeId = g.DangerousGoodTypeFk.BayanIntegrationId,

                            //?  goodsCatgoy or free text or Good Types Description
                            Description = g.Description,
                            Dimensions = g.Dimentions,
                            ItemNumber = "",
                            //? "(items[0].price) must be greater than 0"
                            //todo make it required => can i take it if expired  ?
                            // Abu Rabee3 said det it 1 
                            Price = 1,
                            Quantity = g.Amount,
                            Weight = g.Weight

                        }).ToList(),
                    Vehicle = new Vehicle()
                    {
                        PlateType = x.AssignedTruckFk.PlateTypeFk.BayanIntegrationId,
                        //? some trucks does not have vehicleSequenceNumber example excel imported trucks
                        // todo check it => can i take it if expired  ?
                        SequenceNumber = vehicleSequenceNumber,
                        VehiclePlate = new VehiclePlate()
                        {
                            Number = Regex.Match(x.AssignedTruckFk.PlateNumber.Replace(" ", ""), @"\d+").Value,
                            LeftLetter = x.AssignedTruckFk.PlateNumber.Replace(" ", "").Replace(Regex.Match(x.AssignedTruckFk.PlateNumber.Replace(" ", ""), @"\d+").Value, "").Substring(0, 1),
                            MiddleLetter = x.AssignedTruckFk.PlateNumber.Replace(" ", "").Replace(Regex.Match(x.AssignedTruckFk.PlateNumber.Replace(" ", ""), @"\d+").Value, "").Substring(1, 1),
                            RightLetter = x.AssignedTruckFk.PlateNumber.Replace(" ", "").Replace(Regex.Match(x.AssignedTruckFk.PlateNumber.Replace(" ", ""), @"\d+").Value, "").Substring(2, 1)
                        }
                    },
                    Driver = new Driver()
                    {
                        //? some drivers does not have driverIdentityNumber example excel drivers 
                        // todo check it 
                        IdentityNumber = driverIdentityNumber,
                        Mobile = "+966" + x.AssignedDriverUserFk.PhoneNumber,
                        Name = x.AssignedDriverUserFk.Name
                    },
                    Carrier = new Carrier()
                    {
                        type = "COMPANY",
                        moi = x.ShippingRequestFk.CarrierTenantFk.MoiNumber
                    },
                    TotalFare = Convert.ToInt32(x.VatAmountWithCommission),
                    //? estimated start date or start working date 
                    // ::info "The pickup date must be not earlier than today"
                    PickUpDate = x.StartTripDate.Date.ToString(),
                    PickUpAddress = x.OriginFacilityFk.Address,
                    //? estimated end date or end working date  ?
                    //? (dropOffDate) must not be null
                    // todo  DropOffDate??:DropOffDate:PickUpDate + 1day
                    DropOffDate = x.EndTripDate.HasValue ? x.EndTripDate.Value.Date.ToString() : x.StartTripDate.Date.AddDays(1).ToString(),
                    DropOffAddress = x.DestinationFacilityFk.Address,
                    Tradable = false,
                    ExtraCharges = extraCharges.IsNullOrEmpty() ? "0" : extraCharges,
                    PaymentMethod = "",
                    PaymentComment = "",
                    PaidBy = "Sender"

                }).FirstOrDefaultAsync();

            return new Tuple<RootForCreate, ShippingRequestTrip>(root, trip);
        }
        private async Task<Tuple<RootForEdit, ShippingRequestTrip>> GetRootForEdit(int id)
        {

            ShippingRequestTrip trip = await _shippingRequestTripTripRepository.GetAsync(id);



            var driverIdentityNumber = (await _documentFilesManager.GetDriverIqamaActiveDocumentAsync(trip.AssignedDriverUserId.Value))?.Number;

            var vehicleSequenceNumber = (await _documentFilesManager.GetTruckIstimaraActiveDocumentAsync(trip.AssignedTruckId.Value))?.Number;


            var extraCharges = (await _shippingRequestTripVasRepository.GetAll()
                .Where(x => x.ShippingRequestTripId == trip.Id)
                .SumAsync(x => x.TotalAmountWithCommission))
                .ToString();


            var root = await _shippingRequestTripTripRepository.GetAll()
                .Where(x => x.Id == id)
                .Select(x => new RootForEdit()
                {
                    Id = x.BayanId,
                    Sender = new Sender()
                    {
                        Address = x.ShippingRequestFk.Tenant.Address,
                        Comment = x.Note,
                        Name = x.ShippingRequestFk.Tenant.companyName,
                        //todo: must not be blank
                        //todo: must match \"\\+9665\\d{8}\
                        Phone = "+" + x.ShippingRequestFk.Tenant.MobileNo
                    },
                    Recipient = new Recipient()
                    {
                        Address = x.ShippingRequestFk.CarrierTenantFk.Address,
                        //todo: must not be blank 
                        //todo: must match \"\\+9665\\d{8}\
                        Phone = "+" + x.ShippingRequestFk.CarrierTenantFk.MobileNo,
                        Name = x.ShippingRequestFk.CarrierTenantFk.companyName,
                    },
                    PickUpLocation = new PickUpLocation()
                    {
                        CityName = x.DestinationFacilityFk.CityFk.DisplayName,
                        RegionName = x.DestinationFacilityFk.CityFk.CountyFk.DisplayName
                    },
                    DropOffLocation = new DropOffLocation()
                    {
                        CityName = x.OriginFacilityFk.CityFk.DisplayName,
                        RegionName = x.OriginFacilityFk.CityFk.CountyFk.DisplayName
                    },
                    Items = x.RoutPoints
                        .SelectMany(r => r.GoodsDetails)
                        //? no goodsType ?
                        .Select(g => new Item()
                        {
                            DangerousCode = g.DangerousGoodsCode,
                            DangerousGoodTypeId = g.DangerousGoodTypeFk.BayanIntegrationId,

                            //?  goodsCatgoy or free text or Good Types Description
                            Description = g.Description,
                            Dimensions = g.Dimentions,
                            ItemNumber = "",
                            //? "(items[0].price) must be greater than 0"
                            //todo make it required => can i take it if expired  ?
                            // Abu Rabee3 said det it 1 
                            Price = 1,
                            Quantity = g.Amount,
                            Weight = g.Weight

                        }).ToList(),
                    Vehicle = new Vehicle()
                    {
                        PlateType = x.AssignedTruckFk.PlateTypeFk.BayanIntegrationId,
                        //? some trucks does not have vehicleSequenceNumber example excel imported trucks
                        // todo check it => can i take it if expired  ?
                        SequenceNumber = vehicleSequenceNumber,
                        VehiclePlate = new VehiclePlate()
                        {
                            Number = x.AssignedTruckFk.PlateNumber.Replace(" ", "").Substring(3, 4),
                            LeftLetter = x.AssignedTruckFk.PlateNumber.Replace(" ", "").Substring(0, 1),
                            MiddleLetter = x.AssignedTruckFk.PlateNumber.Replace(" ", "").Substring(1, 1),
                            RightLetter = x.AssignedTruckFk.PlateNumber.Replace(" ", "").Substring(2, 1)
                        }
                    },
                    Driver = new Driver()
                    {
                        //? some drivers does not have driverIdentityNumber example excel drivers 
                        // todo check it 
                        IdentityNumber = driverIdentityNumber,
                        Mobile = "+966" + x.AssignedDriverUserFk.PhoneNumber,
                        Name = x.AssignedDriverUserFk.Name
                    },
                    Carrier = new Carrier()
                    {
                        type = "COMPANY",
                        moi = x.ShippingRequestFk.CarrierTenantFk.MoiNumber
                    },
                    TotalFare = Convert.ToInt32(x.VatAmountWithCommission),
                    //? estimated start date or start working date 
                    // ::info "The pickup date must be not earlier than today"
                    PickUpDate = x.StartTripDate.Date.ToString(),
                    PickUpAddress = x.OriginFacilityFk.Address,
                    //? estimated end date or end working date  ?
                    //? (dropOffDate) must not be null
                    // todo  DropOffDate??:DropOffDate:PickUpDate + 1day
                    DropOffDate = x.EndTripDate.HasValue ? x.EndTripDate.Value.Date.ToString() : x.StartTripDate.Date.AddDays(1).ToString(),
                    DropOffAddress = x.DestinationFacilityFk.Address,
                    Tradable = false,
                    ExtraCharges = extraCharges.IsNullOrEmpty() ? "0" : extraCharges,
                    PaymentMethod = "",
                    PaymentComment = "",
                    PaidBy = "Sender"

                }).FirstOrDefaultAsync();

            return new Tuple<RootForEdit, ShippingRequestTrip>(root, trip);
        }


        private static string ToJsonLowerCaseFirstLetter(object root)
        {
            var body = Newtonsoft.Json.JsonConvert.SerializeObject(root, new JsonSerializerSettings
            { ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver() });
            return body;
        }



    }
}