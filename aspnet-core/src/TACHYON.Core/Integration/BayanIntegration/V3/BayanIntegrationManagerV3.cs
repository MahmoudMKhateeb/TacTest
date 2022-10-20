using Abp.BackgroundJobs;
using Abp.Configuration;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Extensions;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NUglify.Helpers;
using RestSharp;
using Stripe;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TACHYON.Authorization.Users;
using TACHYON.Configuration;
using TACHYON.Documents;
using TACHYON.Integration.BayanIntegration.Modules;
using TACHYON.Integration.BayanIntegration.V3.Jobs;
using TACHYON.Integration.BayanIntegration.V3.Modules;
using TACHYON.Routs.RoutPoints;
using TACHYON.Shipping.ShippingRequests;
using TACHYON.Shipping.ShippingRequestTrips;
using TACHYON.ShippingRequestTripVases;
using TACHYON.Trucks;
using Carrier = TACHYON.Integration.BayanIntegration.V3.Modules.Carrier;
using Driver = TACHYON.Integration.BayanIntegration.V3.Modules.Driver;
using Item = TACHYON.Integration.BayanIntegration.V3.Modules.Item;
using Recipient = TACHYON.Integration.BayanIntegration.V3.Modules.Recipient;
using Sender = TACHYON.Integration.BayanIntegration.V3.Modules.Sender;
using Vehicle = TACHYON.Integration.BayanIntegration.V3.Modules.Vehicle;
using VehiclePlate = TACHYON.Integration.BayanIntegration.V3.Modules.VehiclePlate;

namespace TACHYON.Integration.BayanIntegration.V3
{
    public class UpdateVehicleOrDriverInput
    {
        public string TripId { get; set; }
        public Vehicle Vehicle { get; set; }
        public Driver Driver { get; set; }
        public Driver ExtraDriver { get; set; }
    }

    public class BayanIntegrationManagerV3 : IntegrationDomainServiceBase
    {
        private readonly IRepository<ShippingRequest, long> _shippingRequestRepository;
        private readonly IRepository<ShippingRequestTrip> _shippingRequestTripTripRepository;
        private readonly IRepository<ShippingRequestTripVas, long> _shippingRequestTripVasRepository;
        private readonly DocumentFilesManager _documentFilesManager;
        private readonly ISettingManager _settingManager;
        private readonly IBackgroundJobManager _backgroundJobManager;
        private readonly IRepository<BayanIntegrationResult, long> _bayanIntegrationResultRepository;
        private readonly IRepository<User, long> _userRepository;
        private readonly IRepository<Truck, long> _truckRepository;
        private readonly IRepository<RoutPoint, long> _routPointRepository;

        public BayanIntegrationManagerV3(IRepository<ShippingRequest, long> shippingRequestRepository, IRepository<ShippingRequestTrip> shippingRequestTripTripRepository,
            IRepository<ShippingRequestTripVas, long> shippingRequestTripVasRepository,
            DocumentFilesManager documentFilesManager,
            ISettingManager settingManager,
            IBackgroundJobManager backgroundJobManager,
            IRepository<BayanIntegrationResult, long> bayanIntegrationResultRepository,
            IRepository<User, long> userRepository,
            IRepository<Truck, long> truckRepository,
            IRepository<RoutPoint, long> routPointRepository)
        {
            _shippingRequestRepository = shippingRequestRepository;
            _shippingRequestTripTripRepository = shippingRequestTripTripRepository;
            _shippingRequestTripVasRepository = shippingRequestTripVasRepository;
            _documentFilesManager = documentFilesManager;
            _settingManager = settingManager;
            _backgroundJobManager = backgroundJobManager;
            _bayanIntegrationResultRepository = bayanIntegrationResultRepository;
            _userRepository = userRepository;
            _truckRepository = truckRepository;
            _routPointRepository = routPointRepository;
        }


        private Task<string> Url => _settingManager.GetSettingValueAsync(AppSettings.BayanIntegration.Url);
        private Task<string> AppId => _settingManager.GetSettingValueAsync(AppSettings.BayanIntegration.AppId);
        private Task<string> AppKey => _settingManager.GetSettingValueAsync(AppSettings.BayanIntegration.AppKey);
        private Task<string> ClientId => _settingManager.GetSettingValueAsync(AppSettings.BayanIntegration.ClientId);

        private async Task<IRestResponse> RestClient(string service, Method method, string body)
        {
            var client = new RestClient(await Url + service);
            var request = new RestRequest(method);
            request.AddHeader("app_id", await AppId);
            request.AddHeader("app_key", await AppKey);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("client_id", await ClientId);
            request.AddParameter("application/json", body, ParameterType.RequestBody);
            return await client.ExecuteAsync(request);



        }


        private async Task CreateBayanIntegrationResult(int tripId, string service, string body, IRestResponse response)
        {
            long t = await _bayanIntegrationResultRepository.InsertAndGetIdAsync
            (
                new BayanIntegrationResult
                {
                    ActionName = service,
                    InputJson = body,
                    ResponseJson = response.Content,
                    Version = "3",
                    ShippingRequestTripId = tripId
                }
            );
        }





        /// <summary>
        /// This is service shall be utilized by Freight Forwarders to allows their systems to create Trip in Bayan system
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task CreateTrip(int id)
        {

            using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MayHaveTenant, AbpDataFilters.MustHaveTenant))
            {

                Tuple<Root, ShippingRequestTrip> tuple = await GetRootForCreate(id);

                string body = ToJsonLowerCaseFirstLetter(tuple.Item1);
                var method = Method.POST;
                var Service = "trip";

                IRestResponse response = await RestClient(Service, method, body); ;

                await CreateBayanIntegrationResult(id, Service, body, response);


                if (response.StatusCode == HttpStatusCode.OK)
                {
                    Logger.Info("BayanIntegrationManagerV3.trip" + response.Content);


                    var routePoints = await _shippingRequestTripTripRepository.GetAll()
                        .Where(x => x.Id == id)
                        .SelectMany(x => x.RoutPoints)
                        .Where(x => x.PickingType == PickingType.Dropoff)
                        .OrderBy(r => r.Id)
                        .ToListAsync();

                    var responseRoot = JsonConvert.DeserializeObject<ResponseRoot>(response.Content);

                    tuple.Item2.BayanId = responseRoot.tripId.ToString();


                    foreach (var routePoint in routePoints) // mp bayan waybill ids with point bayanId 
                    {
                        var index = routePoints.IndexOf(routePoint);
                        var elementAtOrDefault = responseRoot.waybills.ElementAtOrDefault(index);
                        routePoint.BayanId = elementAtOrDefault?.waybillId.ToString();
                    }


                }
                else
                {
                    Logger.Error("BayanIntegrationManager.trip" + response.Content);

                }




            }


        }



        /// <summary>
        /// This is service shall be utilized by Freight Forwarders to allows their systems to update vehicle or driver or extra driver
        /// </summary>
        /// <param name="updateVehicleOrDriverInput"></param>
        public async Task UpdateVehicleOrDriver(UpdateVehicleOrDriverJobArgs args)
        {

            using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MayHaveTenant, AbpDataFilters.MustHaveTenant))
            {

                var trip = await _shippingRequestTripTripRepository.GetAsync(args.TripId);

                var vehicle = await _truckRepository.GetAll()
                    .Where(x => x.Id == args.TruckId)
                    .Select
                    (
                        x => new Vehicle
                        {
                            plateTypeId = x.PlateTypeFk.BayanIntegrationId,
                            vehiclePlate = new VehiclePlate
                            {
                                rightLetter = x.PlateNumber.Replace(" ", "").Replace(Regex.Match(x.PlateNumber.Replace(" ", ""), @"\d+").Value, "").Substring(2, 1),
                                middleLetter = x.PlateNumber.Replace(" ", "").Replace(Regex.Match(x.PlateNumber.Replace(" ", ""), @"\d+").Value, "").Substring(1, 1),
                                leftLetter = x.PlateNumber.Replace(" ", "").Replace(Regex.Match(x.PlateNumber.Replace(" ", ""), @"\d+").Value, "").Substring(0, 1),
                                number = Regex.Match(x.PlateNumber.Replace(" ", ""), @"\d+").Value
                            }
                        }
                    ).FirstOrDefaultAsync();



                var driverIdentityNumber = (await _documentFilesManager.GetDriverIqamaActiveDocumentAsync(args.DriverId))?.Number;


                var driver = await _userRepository.GetAsync(args.DriverId);



                UpdateVehicleOrDriverInput input = new UpdateVehicleOrDriverInput
                {
                    TripId = trip.BayanId,
                    Vehicle = vehicle,
                    Driver = new Driver
                    {
                        identityNumber = driverIdentityNumber,
                        issueNumber = driver.DriverIssueNumber.Value,
                        mobile = "+966" + driver.PhoneNumber
                    }
                    //ExtraDriver = null
                };

                string body = ToJsonLowerCaseFirstLetter(input);

                var client = new RestClient(await Url + "trip");
                var request = new RestRequest(Method.PUT);
                request.AddHeader("app_id", await AppId);
                request.AddHeader("app_key", await AppKey);
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("client_id", await ClientId);
                request.AddParameter("application/json", body, ParameterType.RequestBody);
                IRestResponse response = await client.ExecuteAsync(request);

                var t = await _bayanIntegrationResultRepository.InsertAndGetIdAsync
                (
                    new BayanIntegrationResult
                    {
                        ActionName = "BayanIntegrationManagerV3.UpdateVehicleOrDriver",
                        InputJson = body,
                        ResponseJson = response.Content,
                        Version = "3",
                        ShippingRequestTripId = args.TripId,
                    }
                );


                if (response.StatusCode != HttpStatusCode.OK)
                {
                    Logger.Error("BayanIntegrationManagerV3.UpdateVehicleOrDriver" + response.Content);
                }
                else
                {
                    Logger.Trace("BayanIntegrationManager.UpdateVehicleOrDriver" + response.Content);
                }

            }
        }


        public async Task UpdateWaybill(long routPointId)
        {
            using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MayHaveTenant, AbpDataFilters.MustHaveTenant))
            {

                var tripId = (await _routPointRepository.GetAsync(routPointId)).ShippingRequestTripId;



                var extraCharges = (await _shippingRequestTripVasRepository.GetAll()
                        .Where(x => x.ShippingRequestTripId == tripId)
                        .SumAsync(x => x.TotalAmountWithCommission))
                        .ToString();


                var point = _routPointRepository.GetAll()
                    .Where(x => x.Id == routPointId)
                    .Select
                    (p => new Waybill
                    {



                        waybillId = p.BayanId,
                        sender =
                            new Sender
                            {
                                name = p.ShippingRequestTripFk.ShippingRequestFk.Tenant.companyName,
                                phone = "+966" + p.ShippingRequestTripFk.RoutPoints.FirstOrDefault(x => x.PickingType == PickingType.Pickup).ReceiverFk.PhoneNumber,
                                countryCode = p.ShippingRequestTripFk.ShippingRequestFk.Tenant.CountyFk.Code,
                                cityId = p.ShippingRequestTripFk.ShippingRequestFk.Tenant.CityFk.BayanIntegrationId.HasValue ? p.ShippingRequestTripFk.ShippingRequestFk.Tenant.CityFk.BayanIntegrationId.Value.ToString() : "",
                                address = p.ShippingRequestTripFk.ShippingRequestFk.Tenant.Address,
                                notes = ""
                            },
                        recipient =
                            new Recipient
                            {
                                name = p.ShippingRequestTripFk.ShippingRequestFk.CarrierTenantFk.companyName,
                                phone = "+966" + p.ShippingRequestTripFk.RoutPoints.FirstOrDefault(x => x.PickingType == PickingType.Dropoff).ReceiverFk.PhoneNumber,
                                countryCode = p.ShippingRequestTripFk.ShippingRequestFk.CarrierTenantFk.CountyFk.Code,
                                cityId = p.ShippingRequestTripFk.ShippingRequestFk.CarrierTenantFk.CityFk.BayanIntegrationId.HasValue ? p.ShippingRequestTripFk.ShippingRequestFk.CarrierTenantFk.CityFk.BayanIntegrationId.Value.ToString() : "",
                                address = p.ShippingRequestTripFk.ShippingRequestFk.CarrierTenantFk.Address,
                                notes = ""
                            },
                        receivingLocation =
                            new ReceivingLocation
                            {
                                countryCode = p.ShippingRequestTripFk.OriginFacilityFk.CityFk.CountyFk.Code,
                                cityId = p.ShippingRequestTripFk.OriginFacilityFk.CityFk.BayanIntegrationId.Value,
                                address = p.ShippingRequestTripFk.OriginFacilityFk.Address
                            },
                        deliveryLocation = new DeliveryLocation
                        {
                            countryCode = p.ShippingRequestTripFk.DestinationFacilityFk.CityFk.CountyFk.Code,
                            cityId = p.ShippingRequestTripFk.DestinationFacilityFk.CityFk.BayanIntegrationId.Value,
                            address = p.ShippingRequestTripFk.DestinationFacilityFk.Address
                        },
                        deliverToClient = false,
                        items = p.GoodsDetails
                            .Select
                            (
                                g => new Item
                                {
                                    unitId = g.UnitOfMeasureFk.BayanIntegrationId, // todo ask
                                    valid = true, // todo ask 
                                    quantity = g.Amount,
                                    price = "",
                                    goodTypeId = g.GoodCategoryFk.BayanIntegrationId.IfNullOrWhiteSpace(""),
                                    dangerousGoodTypeId = g.DangerousGoodTypeFk.BayanIntegrationId.HasValue ? g.DangerousGoodTypeFk.BayanIntegrationId.Value.ToString() : "",
                                    weight = g.Weight,
                                    dimensions = g.Dimentions.IfNullOrWhiteSpace(""),
                                    dangerousCode = g.DangerousGoodsCode.IfNullOrWhiteSpace(""),
                                    itemNumber = g.Amount.ToString()
                                }
                            ).ToList(),
                        fare = Convert.ToInt32(p.ShippingRequestTripFk.VatAmountWithCommission),
                        tradable = false,
                        extraCharges = extraCharges.IsNullOrEmpty() ? "0" : extraCharges,
                        paymentMethodId = 1, // todo ask
                        paymentComment = "",
                        paidBySender = true
                    });




                string body = ToJsonLowerCaseFirstLetter(point);
                var method = Method.PUT;
                var Service = @"trip/waybill";

                IRestResponse response = await RestClient(Service, method, body); ;

                await CreateBayanIntegrationResult((int)routPointId, Service, body, response);


                if (response.StatusCode == HttpStatusCode.OK)
                {
                    Logger.Info("BayanIntegrationManagerV3.trip/waybill" + response.Content);

                }
                else
                {
                    Logger.Error("BayanIntegrationManager.trip/waybill" + response.Content);

                }




            }
        }

        public async Task CloseWaybill(long routPointId)
        {
            using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MayHaveTenant, AbpDataFilters.MustHaveTenant))
            {

                var point = await _routPointRepository.GetAsync(routPointId);


                if (point.ActualPickupOrDeliveryDate != null)
                {
                    var root = new CloseWaybillRoot { waybillId = point.BayanId, actualDeliveryDate = point.ActualPickupOrDeliveryDate.Value.ToString() };

                    string body = ToJsonLowerCaseFirstLetter(root);
                    var method = Method.PUT;
                    var Service = @"trip/waybill/close";

                    IRestResponse response = await RestClient(Service, method, body); ;

                    await CreateBayanIntegrationResult((int)routPointId, Service, body, response);


                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        Logger.Info("BayanIntegrationManagerV3.trip/waybill/close" + response.Content);

                    }
                    else
                    {
                        Logger.Error("BayanIntegrationManager.trip/waybill/close" + response.Content);

                    }
                }







            }
        }


        public async Task<byte[]> PrintTrip(int id)
        {

            using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MayHaveTenant, AbpDataFilters.MustHaveTenant))
            {
                //try
                //{
                var trip = await _shippingRequestTripTripRepository.GetAsync(id);
                dynamic b = JsonConvert.DeserializeObject(trip.BayanId);

                string tripBayanId;
                try
                {
                   
                        tripBayanId = b.tripId;
                }
                catch
                {
                        tripBayanId  = b.ToString();
                }
             
                //var tripBayanId = "97";

                var client = new RestClient(await Url + "trip/" + tripBayanId + "/print");
                //client.Timeout = -1;
                var request = new RestRequest(Method.GET);
                request.AddHeader("app_id", await AppId);
                request.AddHeader("app_key", await AppKey);
                request.AddHeader("client_id", await ClientId);
                request.AddHeader("Content-Type", "application/pdf");

                var response = client.Execute(request);
                byte[] bytes = response.RawBytes;

                return bytes;






                //var t = await _bayanIntegrationResultRepository.InsertAndGetIdAsync
                //     (
                //         new BayanIntegrationResult
                //         {
                //             ActionName = "BayanIntegrationManagerV3.print",
                //             InputJson = tripBayanId,
                //             ResponseJson = response.Content,
                //             Version = "3",
                //             ShippingRequestTripId = id,
                //         }
                //     );


                //if (response.StatusCode != HttpStatusCode.OK)
                //{
                //    Logger.Error("BayanIntegrationManagerV3.trip" + response.Content);

                //    // throw new Exception("Content: " + response.Content + " , body: " + body);


                //}
                //else
                //{
                //    Logger.Trace("BayanIntegrationManager.CreateConsignmentNote" + response.Content);

                //}

                //return response;


            }


        }



        public async Task QueueCreateTrip(int tripId)
        {
            var queueCreateConsignmentNoteJobId = await _backgroundJobManager.EnqueueAsync<CreateTripJop, int>(tripId);
        }

        public async Task QueueUpdateVehicleOrDriver(UpdateVehicleOrDriverJobArgs args)
        {
            var queueCreateConsignmentNoteJobId = await _backgroundJobManager.EnqueueAsync<UpdateVehicleOrDriverJob, UpdateVehicleOrDriverJobArgs>(args);
        }

        public async Task QueueUpdateWaybill(long routPointId)
        {
            var queue = await _backgroundJobManager.EnqueueAsync<UpdateWaybillJob, long>(routPointId);
        } // todo integrate it in service 

        public async Task QueueCloseWaybillJob(long routPointId)
        {
            var queue = await _backgroundJobManager.EnqueueAsync<CloseWaybillJob, long>(routPointId);
        } // todo integrate it in service 


        private async Task<Tuple<Root, ShippingRequestTrip>> GetRootForCreate(int id)
        {

            ShippingRequestTrip trip = await _shippingRequestTripTripRepository.GetAsync(id);



            var driverIdentityNumber = (await _documentFilesManager.GetDriverIqamaActiveDocumentAsync(trip.AssignedDriverUserId.Value))?.Number;



            //var vehicleSequenceNumber = (await _documentFilesManager.GetTruckIstimaraActiveDocumentAsync(trip.AssignedTruckId.Value))?.Number;


            var extraCharges = (await _shippingRequestTripVasRepository.GetAll()
                .Where(x => x.ShippingRequestTripId == trip.Id)
                .SumAsync(x => x.TotalAmountWithCommission))
                .ToString();


            var root = await _shippingRequestTripTripRepository.GetAll()
                .Where(x => x.Id == id)
                .Select(x => new Root
                {
                    vehicle = new Vehicle
                    {
                        plateTypeId = x.AssignedTruckFk.PlateTypeFk.BayanIntegrationId,
                        vehiclePlate = new VehiclePlate
                        {
                            rightLetter = x.AssignedTruckFk.PlateNumber.Replace(" ", "").Replace(Regex.Match(x.AssignedTruckFk.PlateNumber.Replace(" ", ""), @"\d+").Value, "").Substring(2, 1),
                            middleLetter = x.AssignedTruckFk.PlateNumber.Replace(" ", "").Replace(Regex.Match(x.AssignedTruckFk.PlateNumber.Replace(" ", ""), @"\d+").Value, "").Substring(1, 1),
                            leftLetter = x.AssignedTruckFk.PlateNumber.Replace(" ", "").Replace(Regex.Match(x.AssignedTruckFk.PlateNumber.Replace(" ", ""), @"\d+").Value, "").Substring(0, 1),
                            number = Regex.Match(x.AssignedTruckFk.PlateNumber.Replace(" ", ""), @"\d+").Value
                        }
                    },
                    driver = new Driver
                    {
                        //identityNumber = driverIdentityNumber.IfNullOrWhiteSpace(""),
                        identityNumber = "1000000005",
                        issueNumber = x.AssignedDriverUserFk.DriverIssueNumber.Value, // todo add this
                                                                                      //issueNumber = 11, // todo add this
                        mobile = "+966" + x.AssignedDriverUserFk.PhoneNumber
                    },
                    //extraDriver = new ExtraDriver
                    //{
                    //    identityNumber = "",
                    //    issueNumber = "",
                    //    mobile = ""
                    //},
                    carrier = new Carrier
                    {
                        type = "COMPANY",
                        moi = x.ShippingRequestFk.CarrierTenantFk.MoiNumber
                    },
                    receivedDate = x.StartTripDate.Date.ToString(),
                    //receivedDate = DateTime.Now.Date.ToString("yyyy-MM-dd"),
                    expectedDeliveryDate = x.EndTripDate.HasValue ? x.EndTripDate.Value.Date.ToString() : x.StartTripDate.Date.AddDays(1).ToString(),
                    //expectedDeliveryDate = DateTime.Now.Date.AddDays(1).ToString("yyyy-MM-dd"),
                    notes = "",
                    waybills = x.RoutPoints
                        .Where(x => x.PickingType == PickingType.Dropoff)
                        .OrderBy(r => r.Id)
                        .Select(p => new Waybill
                        {
                            sender = new Sender
                            {
                                name = x.ShippingRequestFk.Tenant.companyName,
                                phone = "+966" + x.RoutPoints.FirstOrDefault(x => x.PickingType == PickingType.Pickup).ReceiverFk.PhoneNumber,
                                countryCode = x.ShippingRequestFk.Tenant.CountyFk.Code,
                                cityId = x.ShippingRequestFk.Tenant.CityFk.BayanIntegrationId.HasValue ? x.ShippingRequestFk.Tenant.CityFk.BayanIntegrationId.Value.ToString() : "",
                                address = x.ShippingRequestFk.Tenant.Address,
                                notes = ""
                            },
                            recipient = new Recipient
                            {
                                name = x.ShippingRequestFk.CarrierTenantFk.companyName,
                                phone = "+966" + x.RoutPoints.FirstOrDefault(x => x.PickingType == PickingType.Dropoff).ReceiverFk.PhoneNumber,
                                countryCode = x.ShippingRequestFk.CarrierTenantFk.CountyFk.Code,
                                cityId = x.ShippingRequestFk.CarrierTenantFk.CityFk.BayanIntegrationId.HasValue ? x.ShippingRequestFk.CarrierTenantFk.CityFk.BayanIntegrationId.Value.ToString() : "",
                                address = x.ShippingRequestFk.CarrierTenantFk.Address,
                                notes = ""
                            },
                            receivingLocation = new ReceivingLocation
                            {
                                countryCode = x.OriginFacilityFk.CityFk.CountyFk.Code,
                                cityId = x.OriginFacilityFk.CityFk.BayanIntegrationId.Value,
                                address = x.OriginFacilityFk.Address
                            },
                            deliveryLocation = new DeliveryLocation
                            {
                                countryCode = x.DestinationFacilityFk.CityFk.CountyFk.Code,
                                cityId = x.DestinationFacilityFk.CityFk.BayanIntegrationId.Value,
                                address = x.DestinationFacilityFk.Address
                            },
                            deliverToClient = false,

                            items = p.GoodsDetails
                                    .Select(g => new Item
                                    {
                                        unitId = g.UnitOfMeasureFk.BayanIntegrationId, // todo ask
                                        valid = true, // todo ask 
                                        quantity = g.Amount,
                                        price = "",
                                        goodTypeId = g.GoodCategoryFk.BayanIntegrationId.IfNullOrWhiteSpace(""),
                                        dangerousGoodTypeId = g.DangerousGoodTypeFk.BayanIntegrationId.HasValue ? g.DangerousGoodTypeFk.BayanIntegrationId.Value.ToString() : "",
                                        weight = g.Weight,
                                        dimensions = g.Dimentions.IfNullOrWhiteSpace(""),
                                        dangerousCode = g.DangerousGoodsCode.IfNullOrWhiteSpace(""),
                                        itemNumber = g.Amount.ToString()
                                    }
                                    ).ToList(),
                            fare = Convert.ToInt32(x.VatAmountWithCommission),
                            tradable = false,
                            extraCharges = extraCharges.IsNullOrEmpty() ? "0" : extraCharges,
                            paymentMethodId = 1, // todo ask
                            paymentComment = "",
                            paidBySender = true
                        }
                        ).ToList()
                }
                ).FirstOrDefaultAsync();



            return new Tuple<Root, ShippingRequestTrip>(root, trip);
        }




    }


    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class ResponseRoot
    {
        public int tripId { get; set; }
        public List<ResponseWaybill> waybills { get; set; }
    }

    public class ResponseWaybill
    {
        public int waybillId { get; set; }
        public string senderName { get; set; }
        public string senderFullAddress { get; set; }
        public string recipientName { get; set; }
        public string recipientFullAddress { get; set; }
    }

    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class CloseWaybillRoot

    {
        public string waybillId { get; set; }
        public string actualDeliveryDate { get; set; }
    }





}
