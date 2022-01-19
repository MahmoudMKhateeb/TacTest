using Abp.BackgroundJobs;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Json;
using Abp.Timing;
using Microsoft.EntityFrameworkCore;
using RestSharp;
using System;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using TACHYON.Authorization.Users;
using TACHYON.Documents;
using TACHYON.Integration.WaslIntegration.Jobs;
using TACHYON.Integration.WaslIntegration.Modules;
using TACHYON.Routs.RoutPoints;
using TACHYON.Shipping.ShippingRequestTrips;
using TACHYON.Trucks;

namespace TACHYON.Integration.WaslIntegration
{
    public class WaslIntegrationManager : IntegrationDomainServiceBase
    {
        private readonly IBackgroundJobManager _backgroundJobManager;
        private readonly IRepository<ShippingRequestTrip> _shippingRequestTripRepository;
        private readonly DocumentFilesManager _documentFilesManager;


        public WaslIntegrationManager(IBackgroundJobManager backgroundJobManager, IRepository<ShippingRequestTrip> shippingRequestTripRepository, DocumentFilesManager documentFilesManager)
        {
            _backgroundJobManager = backgroundJobManager;
            _shippingRequestTripRepository = shippingRequestTripRepository;
            _documentFilesManager = documentFilesManager;
        }

        /// <summary>
        /// This is service shall be utilized in case of 
        /// registering a new vehicle if the vehicle are not
        /// previously registered in Wasl Platform, it is 
        /// also utilized for cases that requires reregistration/updating information.        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task VehicleRegistration(Truck input)
        {
            var response = await Vehicle(input, Method.POST);
        }

        /// <summary>
        /// This service shall be utilized by Electronic 
        /// Freight Forwarders to delete a single vehicle
        /// previously registered in Wasl Platform
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task VehicleDelete(Truck input)
        {
            var response = await Vehicle(input, Method.DELETE);
        }

        /// <summary>
        /// This is service shall be utilized in case of 
        /// registering a new Driver if the Driver is not
        /// previously registered in Wasl Platform, it is 
        /// also utilized for cases that requires reregistration/updating information.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task DriverRegistration(User input)
        {
            await Drivers(input, Method.POST);
        }

        /// <summary>
        /// This service shall be utilized by Electronic 
        /// Freight Forwarders to delete a single Driver
        /// previously registered in Wasl Platform.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task DriverDelete(User input)
        {
            await Drivers(input, Method.DELETE);
        }


        private async Task<IRestResponse> Vehicle(Truck input, Method method)
        {
            using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MayHaveTenant, AbpDataFilters.MustHaveTenant))
            {
                var vehicleSequenceNumber = (await _documentFilesManager.GetTruckIstimaraActiveDocumentAsync(input.Id))?.Number;
                var root = new WaslVehicleRoot()
                {
                    Plate = NormalizePlateNumber(input.PlateNumber),
                    PlateType = 2,
                    SequenceNumber = vehicleSequenceNumber.ToString()
                };

                var request = new RestRequest(method);
                WaslRequestAddheaders(request);
                string body = ToJsonLowerCaseFirstLetter(root);
                var client = new RestClient("https://wasl.api.elm.sa/api/eff/v1/vehicles");
                // client.Timeout = -1;


                request.AddParameter("application/json", body, ParameterType.RequestBody);
                IRestResponse response = await client.ExecuteAsync(request);
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    Logger.Error("WaslIntegrationManager.Vehicle" + response.Content);
                    input.WaslIntegrationErrorMsg = response.Content;
                    throw new Exception("Content: " + response.Content + " , body: " + body);

                }
                else
                {
                    Logger.Trace("WaslIntegrationManager.Vehicle" + response.Content);

                    input.IsWaslIntegrated = (method == Method.POST);
                }
                return response;

            }
        }



        private async Task Drivers(User input, Method method)
        {
            using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MayHaveTenant, AbpDataFilters.MustHaveTenant))
            {

                WaslDriversRoot root = new WaslDriversRoot()
                {
                    DateOfBirthGregorian = input.DateOfBirth.Value.ToString("yyyy-MM-dd"),
                    DateOfBirthHijri = "",
                    Email = input.EmailAddress,
                    IdentityNumber = (await _documentFilesManager.GetDriverIqamaActiveDocumentAsync(input.Id))?.Number,
                    MobileNumber = "+966" + input.PhoneNumber
                };



                var client = new RestClient("https://wasl.api.elm.sa/api/eff/v1/drivers");
                client.Timeout = -1;
                var request = new RestRequest(method);
                WaslRequestAddheaders(request);
                string body = ToJsonLowerCaseFirstLetter(root);

                request.AddParameter("application/json", body, ParameterType.RequestBody);
                IRestResponse response = await client.ExecuteAsync(request);
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    Logger.Error("WaslIntegrationManager.Drivers" + response.Content);
                    throw new Exception("Content: " + response.Content + " , body: " + body);

                }
                else
                {
                    Logger.Trace("WaslIntegrationManager.Drivers" + response.Content);
                }

            }
        }


        /// <summary>
        /// This service shall be utilized by Electronic Freight Forwarders to send the trip information to Wasl Platform.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task TripRegistration(int tripId)
        {
            using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MayHaveTenant, AbpDataFilters.MustHaveTenant))
            {
                ShippingRequestTrip trip = await _shippingRequestTripRepository.GetAsync(tripId);

                var root = await _shippingRequestTripRepository.GetAll()
                    .Where(x => x.Id == tripId)
                    .Select(trip => new WaslTripRoot
                    {
                        DepartedWhen = trip.StartWorking.HasValue ? trip.StartWorking.Value.ToString("yyyy-MM-ddTHH:mm:ss") : DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"),
                        DepartureLatitude = trip.OriginFacilityFk.Location.Y,
                        DepartureLongitude = trip.OriginFacilityFk.Location.X,
                        ExpectedDestinationLatitude = trip.DestinationFacilityFk.Location.Y,
                        ExpectedDestinationLongitude = trip.DestinationFacilityFk.Location.X,
                        TripNumber = tripId.ToString()
                    }).FirstOrDefaultAsync();


                //fill driver and truck numbers
                root.DriverIdentityNumber = (await _documentFilesManager.GetDriverIqamaActiveDocumentAsync(trip.AssignedDriverUserId.Value))?.Number;
                root.VehicleSequenceNumber = (await _documentFilesManager.GetTruckIstimaraActiveDocumentAsync(trip.AssignedTruckId.Value))?.Number;

                using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MayHaveTenant, AbpDataFilters.MustHaveTenant))
                {

                    var client = new RestClient("https://wasl.api.elm.sa/api/eff/v1/trips");
                    client.Timeout = -1;
                    var request = new RestRequest(Method.POST);
                    WaslRequestAddheaders(request);
                    string body = ToJsonLowerCaseFirstLetter(root);

                    request.AddParameter("application/json", body, ParameterType.RequestBody);
                    IRestResponse response = await client.ExecuteAsync(request);
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        Logger.Error("WaslIntegrationManager.TripRegistration" + response.Content);
                        trip.WaslIntegrationErrorMsg = response.Content;

                        throw new Exception("Content: " + response.Content + " , body: " + body);

                    }
                    else
                    {
                        trip.IsWaslIntegrated = true;
                        Logger.Trace("WaslIntegrationManager.TripRegistration" + response.Content);
                    }

                }
            }


        }

        /// <summary>
        /// This service shall be utilized by Electronic Freight Forwarders to send the latitude, longitude of the current location of the vehicles when the trip has ended and other information to WASL.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="tripId"></param>
        /// <returns></returns>
        public async Task TripUpdate(int tripId)
        {
            ShippingRequestTrip trip = await _shippingRequestTripRepository.GetAsync(tripId);
            var root = new WaslTripUpdateRoot()
            {
                ActualDestinationLatitude = trip.DestinationFacilityFk.Location.Y,
                ActualDestinationLongitude = trip.DestinationFacilityFk.Location.X,
                ArrivedWhen = trip.ActualDeliveryDate.Value.ToString("yyyy-MM-ddTHH:mm:ss")
            };


            using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MayHaveTenant, AbpDataFilters.MustHaveTenant))
            {

                var client = new RestClient("https://wasl.api.elm.sa/api/eff/v1/trips/" + tripId.ToString());
                client.Timeout = -1;
                var request = new RestRequest(Method.PATCH);
                WaslRequestAddheaders(request);
                string body = ToJsonLowerCaseFirstLetter(root);

                request.AddParameter("application/json", body, ParameterType.RequestBody);
                IRestResponse response = await client.ExecuteAsync(request);
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    Logger.Error("WaslIntegrationManager.TripRegistration" + response.Content);
                    trip.WaslIntegrationErrorMsg = response.Content;
                    throw new Exception("Content: " + response.Content + " , body: " + body);
                }
                else
                {
                    trip.IsWaslIntegrated = true;
                    Logger.Trace("WaslIntegrationManager.TripRegistration" + response.Content);
                }

            }
        }


        #region Jobs

        public async Task QueueVehicleRegistrationJob(Truck input)
        {
            // stop json loop Using 

            await _backgroundJobManager.EnqueueAsync<VehicleRegistrationJob, Truck>(input);
        }

        public async Task QueueVehicleDeleteJob(Truck input)
        {

            await _backgroundJobManager.EnqueueAsync<VehicleDeleteJob, Truck>(input);
        }
        public async Task QueueDriverRegistrationJob(User user)
        {
            //just for drivers
            if (!user.IsDriver) return;
            await _backgroundJobManager.EnqueueAsync<DriverRegistrationJob, User>(user);
        }
        public async Task QueueDriverDeleteJob(User user)
        {
            //just for drivers
            if (!user.IsDriver) return;
            await _backgroundJobManager.EnqueueAsync<DriverDeleteJob, User>(user);
        }
        public async Task QueueTripRegistrationJob(int tripId)
        {
            await _backgroundJobManager.EnqueueAsync<TripRegistrationJob, int>(tripId);
        }
        public async Task QueueTripUpdateJob(int tripId)
        {

            await _backgroundJobManager.EnqueueAsync<TripUpdateJob, int>(tripId);
        }

        #endregion

        /// <summary>
        /// <see href="https://en.m.wikipedia.org/wiki/Vehicle_registration_plates_of_Saudi_Arabia">HERE</see>
        /// </summary>
        /// <param name="plateNumber"></param>
        /// <returns></returns>
        private string NormalizePlateNumber(string plateNumber)
        {

            var plate = plateNumber.ToUpper()
                .Replace("A", "ا")
                .Replace("أ", "ا")
                .Replace("إ", "ا")
               .Replace("B", "ب")
               .Replace("J", "ح")
               .Replace("D", "د")
               .Replace("R", "ر")
               .Replace("S", "س")
               .Replace("X", "ص")
               .Replace("T", "ط")
               .Replace("E", "ع")
               .Replace("G", "ق")
               .Replace("K", "ك")
               .Replace("L", "ل")
               .Replace("Z", "م")
               .Replace("N", "ن")
               .Replace("H", "ه")
               .Replace("U", "و")
               .Replace("V", "ى");

            plate = plate.Replace(" ", "");
            plate = plate.Replace("-", "");

            var numbers = String.Join("", plate.Take(4));
            var letters = String.Join("", plate.Skip(4).Take(3));
            plate = letters + numbers;


            plate = plate.Insert(1, " ");
            plate = plate.Insert(3, " ");
            plate = plate.Insert(5, " ");

            return plate;


        }

        private void WaslRequestAddheaders(RestRequest request)
        {
            request.AddHeader("app-id", "4b785cf2");
            request.AddHeader("app-key", "d3a2ad0d2467412384459ebf29b86788");
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("client-id", "52efa1c2-5623-43e2-aacf-b1b9d7ddf8f5");
        }


    }
}