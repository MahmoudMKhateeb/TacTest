using Abp.Application.Services;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Json;
using Microsoft.EntityFrameworkCore;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TACHYON.Documents;
using TACHYON.Routs.RoutPoints;
using TACHYON.Shipping.ShippingRequests;
using TACHYON.Shipping.ShippingRequestTrips;
using TACHYON.ShippingRequestTripVases;

namespace TACHYON.BayanIntegration
{
    public class BayanIntegrationService : ApplicationService
    {
        private readonly IRepository<ShippingRequest, long> _shippingRequestRepository;
        private readonly IRepository<ShippingRequestTrip> _shippingRequestTripTripRepository;
        private readonly IRepository<ShippingRequestTripVas, long> _shippingRequestTripVasRepository;
        private readonly DocumentFilesManager _documentFilesManager;

        public BayanIntegrationService(IRepository<ShippingRequest, long> shippingRequestRepository, IRepository<ShippingRequestTrip> shippingRequestTripTripRepository, DocumentFilesManager documentFilesManager, IRepository<ShippingRequestTripVas, long> shippingRequestTripVasRepository)
        {
            _shippingRequestRepository = shippingRequestRepository;
            _shippingRequestTripTripRepository = shippingRequestTripTripRepository;
            _documentFilesManager = documentFilesManager;
            _shippingRequestTripVasRepository = shippingRequestTripVasRepository;
        }

        /// <summary>
        /// This is service shall be utilized by Electronic Freight Forwarders to allows their systems to create Consignment note in Bayan system. 
        /// </summary>
        public async Task<Root> CreateConsignmentNote(int id)
        {
            var root = await GetRoot(id);
            return root;
            //return Send(root);
        }

        /// <summary>
        /// This is service shall be utilized by Electronic Freight Forwarders to allows their systems to  update Consignment note in Bayan system 
        /// </summary>
        public void EditConsignmentNote()
        {

        }

        /// <summary>
        /// This is service shall be utilized by Electronic Freight Forwarders to allows their systems to  close Consignment note in Bayan system 
        /// </summary>
        public void CloseConsignmentNote()
        {

        }

        private async Task<Root> GetRoot(int id)
        {

            var trip = await _shippingRequestTripTripRepository.GetAsync(id);



            var driverIdentityNumber = (await _documentFilesManager.GetDriverIqamaActiveDocumentAsync(trip.AssignedDriverUserId.Value))?.Number;



            var vehicleSequenceNumber = (await _documentFilesManager.GetTruckIstimaraActiveDocumentAsync(trip.AssignedTruckId.Value))?.Number;


            var extraCharges = (await _shippingRequestTripVasRepository.GetAll()
                .Where(x => x.ShippingRequestTripId == trip.Id)
                .SumAsync(x => x.TotalAmountWithCommission))
                .ToString();


            var root = await _shippingRequestTripTripRepository.GetAll()
                .Where(x => x.Id == id)
                .Select(x => new Root()
                {
                    Sender = new Sender()
                    {
                        Address = x.ShippingRequestFk.Tenant.Address,
                        Comment = x.Note,
                        Name = x.ShippingRequestFk.Tenant.companyName,
                        //todo: must not be blank
                        //todo: must match \"\\+9665\\d{8}\
                        Phone = "+966"+x.ShippingRequestFk.Tenant.MobileNo
                    },
                    Recipient = new Recipient()
                    {
                        Address = x.ShippingRequestFk.CarrierTenantFk.Address,
                        //todo: must not be blank 
                        //todo: must match \"\\+9665\\d{8}\
                        Phone = "+966"+x.ShippingRequestFk.CarrierTenantFk.MobileNo,
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
                            DangerousGoodTypeId = null,

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
                        PlateType = "1",
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
                        Mobile = x.AssignedDriverUserFk.PhoneNumber,
                        Name = x.AssignedDriverUserFk.Name
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
                    ExtraCharges = extraCharges,
                    PaymentMethod = "",
                    PaymentComment = "",
                    PaidBy = "Sender"

                }).FirstOrDefaultAsync();

            return root;
        }

        private string Send(Root root)
        {
            var client = new RestClient("https://bayan.api.elm.sa/api/v1/eff/consignment-notes");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("app_id", "431b4bd1");
            request.AddHeader("app_key", "d4738b317b9fa32a95ec65a39e84adbd");
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("client_id", "56beeab2-d96a-4afd-baed-e4a88894629e");
            //request.AddHeader("Cookie", "NSC_Q-PDQ-3tdbmf_qspevdujpo-TTM=5ccba3d82e1dfb2c99c5d0554ea7a4f932223e9da2adb3d6974724963dda60cc1f0611c1; TS01e2737f=01987f91d384ef676dcfc8b83e0ea812889dd2d603dc030cb5ceb70f435ac16ed48b68ad77dcb34e22ab5fda769b7149d775bc9c93430d5d810488038403935618b287b801521ed1f2ab58b309fa4f33ca071d89cd; ddcb7a40eaf7373e607e39721feb07af=8da15bd2a9ee93a036c66dd184249e13");
            var body = root.ToJsonString();
            request.AddParameter("application/json", body, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            return response.Content;
        }


        public class Sender
        {
            public string Name { get; set; }
            public string Phone { get; set; }
            public string Address { get; set; }
            /// <summary>
            /// Not Required
            /// </summary>
            public string Comment { get; set; }
        }

        public class Recipient
        {
            public string Name { get; set; }
            public string Phone { get; set; }
            public string Address { get; set; }
            /// <summary>
            /// Not Required
            /// </summary>
            public string Comment { get; set; }
        }

        public class PickUpLocation
        {
            public string CityName { get; set; }
            public string RegionName { get; set; }
        }

        public class DropOffLocation
        {
            public string CityName { get; set; }
            public string RegionName { get; set; }
        }

        public class Item
        {
            public int Quantity { get; set; }
            public double Price { get; set; }
            public string Description { get; set; }
            /// <summary>
            /// Not Required
            /// </summary>
            public int? DangerousGoodTypeId { get; set; }
            public double Weight { get; set; }
            /// <summary>
            /// Not Required
            /// </summary>
            public string Dimensions { get; set; }
            /// <summary>
            /// Not Required
            /// </summary>
            public string DangerousCode { get; set; }
            /// <summary>
            /// Not Required
            /// </summary>
            public string ItemNumber { get; set; }
        }

        public class VehiclePlate
        {
            public string RightLetter { get; set; }
            public string MiddleLetter { get; set; }
            public string LeftLetter { get; set; }
            public string Number { get; set; }
        }

        public class Vehicle
        {
            /// <summary>
            /// Vehicle’s sequence number
            /// Istemara number in tachyon 
            /// </summary>
            public string SequenceNumber { get; set; }
            public string PlateType { get; set; }
            /// <summary>
            /// Vehicle’s plate
            /// rightLetter + middleLetter + leftLetter + umber
            /// </summary>
            public VehiclePlate VehiclePlate { get; set; }
        }

        public class Driver
        {
            public string Name { get; set; }
            public string IdentityNumber { get; set; }
            /// <summary>
            /// Not Required
            /// </summary>
            public string Mobile { get; set; }
        }

        public class Root
        {
            public Sender Sender { get; set; }
            public Recipient Recipient { get; set; }
            public PickUpLocation PickUpLocation { get; set; }
            public DropOffLocation DropOffLocation { get; set; }
            public List<Item> Items { get; set; }
            public Vehicle Vehicle { get; set; }
            public Driver Driver { get; set; }
            public int TotalFare { get; set; }
            public string PickUpDate { get; set; }
            public string PickUpAddress { get; set; }
            public string DropOffDate { get; set; }
            public string DropOffAddress { get; set; }

            /// <summary>
            /// always will be False
            /// </summary>
            public bool Tradable { get; set; } = false;
            public string ExtraCharges { get; set; }
            /// <summary>
            /// Not Required
            /// </summary>
            public string PaymentMethod { get; set; }
            /// <summary>
            /// Not Required
            /// </summary>
            public string PaymentComment { get; set; }

            /// <summary>k
            /// paidBy either sender or recipient
            /// always will be the Sender in Tachyon 
            /// </summary>
            public string PaidBy { get; set; } = "Sender";
        }

        public enum BayanPlateTypesEnum
        {
            PublicTransport = 1,
            PrivateTransport = 2,
            HeavyEquipment = 3,
            Export = 4,
            Temporary = 5        }

    }
}
