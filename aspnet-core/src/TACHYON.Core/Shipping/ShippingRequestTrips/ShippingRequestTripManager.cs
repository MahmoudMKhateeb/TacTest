using Abp.Application.Features;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.Runtime.Session;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;
using TACHYON.AddressBook;
using TACHYON.Authorization.Users;
using TACHYON.Features;
using TACHYON.Goods.Dtos;
using TACHYON.Notifications;
using TACHYON.Receivers;
using TACHYON.Routs.RoutPoints;
using TACHYON.Shipping.Dedicated;
using TACHYON.Shipping.ShippingRequests;
using TACHYON.Shipping.Trips;
using TACHYON.Shipping.Trips.Dto;
using TACHYON.ShippingRequestVases;
using TACHYON.Tracking;
using TACHYON.Trucks;

namespace TACHYON.Shipping.ShippingRequestTrips
{
    public class ShippingRequestTripManager: TACHYONDomainServiceBase
    {
        private readonly IRepository<ShippingRequestTrip> _shippingRequestTripRepository;
        private readonly IRepository<ShippingRequest,long> _shippingRequestRepository;
        private readonly IRepository<RoutPoint, long> _routePointRepository;
        private readonly IFeatureChecker _featureChecker;
        private readonly IRepository<Facility, long> _facilityRepository;
        private readonly IRepository<Receiver> _receiverRepository;
        private readonly IAppNotifier _appNotifier;
        private readonly IRepository<ShippingRequestVas, long> _shippingRequestVasRepository;
        private readonly ShippingRequestPointWorkFlowProvider _shippingRequestPointWorkFlowProvider;
        private readonly IRepository<User, long> _userRepository;
        private readonly IRepository<Truck, long> _truckRepository;
        private readonly IRepository<DedicatedShippingRequestDriver, long> _dedicatedShippingRequestDriverRepository;
        private readonly IRepository<DedicatedShippingRequestTruck, long> _dedicatedShippingRequestTruckRepository;

        private IAbpSession _AbpSession { get; set; }


        public ShippingRequestTripManager(IRepository<ShippingRequestTrip> shippingRequestTripRepository,
            IRepository<ShippingRequest, long> shippingRequestRepository, 
            IFeatureChecker featureChecker, IAbpSession abpSession, 
            IRepository<RoutPoint, long> routePointRepository,
            IRepository<Facility, long> facilityRepository,
            IRepository<Receiver> receiverRepository, 
            IAppNotifier appNotifier,
            IRepository<ShippingRequestVas, long> shippingRequestVasRepository, 
            ShippingRequestPointWorkFlowProvider shippingRequestPointWorkFlowProvider,
            UserManager userManager, 
            IRepository<DedicatedShippingRequestDriver, long> dedicatedShippingRequestDriver,
            IRepository<DedicatedShippingRequestTruck, long> dedicatedShippingRequestTruck,
            IRepository<User, long> userRepository, IRepository<Truck, long> truckRepository)
        {
            _shippingRequestTripRepository = shippingRequestTripRepository;
            _shippingRequestRepository = shippingRequestRepository;
            _featureChecker = featureChecker;
            _AbpSession = abpSession;
            _routePointRepository = routePointRepository;
            _facilityRepository = facilityRepository;
            _receiverRepository = receiverRepository;
            _appNotifier = appNotifier;
            _shippingRequestVasRepository = shippingRequestVasRepository;
            _shippingRequestPointWorkFlowProvider = shippingRequestPointWorkFlowProvider;
            _dedicatedShippingRequestDriverRepository = dedicatedShippingRequestDriver;
            _dedicatedShippingRequestTruckRepository = dedicatedShippingRequestTruck;
            _userRepository = userRepository;
            _truckRepository = truckRepository;
        }

        public async Task DriverAcceptTrip(ShippingRequestTrip trip )
        {
            trip.DriverStatus = ShippingRequestTripDriverStatus.Accepted;
            await _shippingRequestPointWorkFlowProvider.TransferPricesToTrip(trip);
            var currentUser = await _shippingRequestPointWorkFlowProvider.GetCurrentUserAsync();
            if (currentUser.IsDriver) await _appNotifier.DriverAcceptTrip(trip, currentUser.FullName);

        }
        public async Task<int> CreateAndGetIdAsync(ShippingRequestTrip trip)
        {
            //var existedTrip = await _shippingRequestTripRepository.FirstOrDefaultAsync(x => x.BulkUploadRef == trip.BulkUploadRef);
            //if (existedTrip != null)
            //{
             //   throw new UserFriendlyException(L("TripAlreadyExists"));
            //}

            return await _shippingRequestTripRepository.InsertAndGetIdAsync(trip);
        }

        public async Task ValidateNumberOfTrips(ShippingRequest request, int tripsNo)
        {
            int requestNumberOfTripsAdd = await _shippingRequestTripRepository.GetAll()
                    .Where(x => x.ShippingRequestId == request.Id).CountAsync() + tripsNo;
            if (requestNumberOfTripsAdd > request.NumberOfTrips)
                throw new UserFriendlyException(L("The number of trips " + request.NumberOfTrips));
        }

        public async Task<long> CreatePointAsync(RoutPoint point)
        {
            return await _routePointRepository.InsertAndGetIdAsync(point);
        }

        public void ValidateTripDto(ImportTripDto importTripDto, StringBuilder exceptionMessage)
        {
            DisableTenancyFilters();
            var SR = GetShippingRequestByPermission(importTripDto.ShippingRequestId);

            if(SR.ShippingRequestFlag == ShippingRequestFlag.Normal)
            {
                if(importTripDto.StartTripDate?.Date == null)
                {
                    importTripDto.StartTripDate = SR.StartTripDate;
                }
                if (importTripDto.EndTripDate != null && importTripDto.StartTripDate?.Date > importTripDto.EndTripDate.Value.Date)
                {
                    exceptionMessage.Append("The start date must be less or equal to end date." + "; ");
                }

                try
                {
                    ValidateTripDates(importTripDto, SR);
                }
                catch (UserFriendlyException e)
                {
                    exceptionMessage.Append(e.Message+";");
                }
            }
            else //dedicated
            {
                if (importTripDto.StartTripDate?.Date == null)
                {
                    importTripDto.StartTripDate = SR.RentalStartDate;
                }
                if (importTripDto.EndTripDate != null && importTripDto.StartTripDate?.Date > importTripDto.EndTripDate.Value.Date)
                {
                    exceptionMessage.Append("The start date must be less or equal to end date." + "; ");
                }

                try
                {
                    ValidateDedicatedRequestTripDates(importTripDto, SR);
                }
                catch (UserFriendlyException e)
                {
                    exceptionMessage.Append(e.Message + ";");
                }

                //number of drops
                if(importTripDto.RouteType == ShippingRequestRouteType.SingleDrop)
                {
                    importTripDto.NumberOfDrops = 1;
                }

                //validate truck and driver
               // await ValidateTruckAndDriver(importTripDto);
            }


            ValidateDuplicateBulkReferenceFromDB(importTripDto, exceptionMessage);
            //ValidateDuplicatedReference(importTripDtoList, exceptionMessage);
            //check if no duplication in reference from DB
            importTripDto.Exception = exceptionMessage.ToString();
        }

        public ShippingRequest GetShippingRequestByPermission(long id)
        {
            DisableTenancyFilters();
            return _shippingRequestRepository.GetAll()
                .Include(x=>x.ShippingRequestVases)
                .WhereIf(_featureChecker.IsEnabled(AppFeatures.TachyonDealer), x => x.IsTachyonDeal)
                .WhereIf(!_featureChecker.IsEnabled(AppFeatures.TachyonDealer), x => x.TenantId == _AbpSession.TenantId)
                .FirstOrDefault(x => x.Id == id);
        }

        public ShippingRequest GetShippingRequestByPermission(int tripId)
        {
            DisableTenancyFilters();
            var trip= _shippingRequestTripRepository.GetAll()
                .Include(x=>x.ShippingRequestFk)
                .WhereIf(_featureChecker.IsEnabled(AppFeatures.TachyonDealer), x => x.ShippingRequestFk.IsTachyonDeal)
                .WhereIf(_featureChecker.IsEnabled(AppFeatures.Shipper), x => x.ShippingRequestFk.TenantId == _AbpSession.TenantId)
                .FirstOrDefault(x => x.Id == tripId);
            if (trip != null)
            {
                return trip.ShippingRequestFk;
            }
            return null;
        }

        public List<string> DuplicatedReferenceFromList(List<string> list)
        {
            if (list != null && list.Count > 0)
            {
                return list.GroupBy(x => x)
                                        .Where(g => g.Count() > 1)
                                        .Select(x => x.Key).ToList();
                
                //List<string> dupicatedRedTripsList = new List<string>();
                //foreach (var trip in importTripDtoList)
                //{
                //    if (importTripDtoList.Count(trip) > 1)
                //    {
                //            dupicatedRedTripsList.Add(trip);
                //    }
                //}
                //return dupicatedRedTripsList;
            }
            return null;
        }

        public void ValidateTripDates(ICreateOrEditTripDtoBase input, ShippingRequest request)
        {
            if (
                input.StartTripDate?.Date > request.EndTripDate?.Date ||
                input.StartTripDate?.Date < request.StartTripDate?.Date ||
                (input.EndTripDate != null && input.EndTripDate.Value.Date > request.EndTripDate?.Date) ||
                (input.EndTripDate != null && input.EndTripDate.Value.Date < request.StartTripDate?.Date)
            )
            {
                throw new UserFriendlyException(L("TheTripDateRangeMustBetweenShippingRequestRangeDate"));
            }
        }

        public void ValidateDedicatedRequestTripDates(ICreateOrEditTripDtoBase input, ShippingRequest request)
        {
            if (
                input.StartTripDate?.Date > request.RentalEndDate?.Date ||
                input.StartTripDate?.Date < request.RentalStartDate?.Date ||
                (input.EndTripDate != null && input.EndTripDate.Value.Date > request.RentalEndDate?.Date) ||
                (input.EndTripDate != null && input.EndTripDate.Value.Date < request.RentalStartDate?.Date)
            )
            {
                throw new UserFriendlyException(L("TheTripDateRangeMustBetweenDedicatedShippingRequestRentalDate"));
            }
        }

        public void ValidateNumberOfDrops(int dropsCount, ShippingRequest request)
        {
            if (dropsCount != request.NumberOfDrops)
            {
                throw new UserFriendlyException(L("The number of drop points must be" + request.NumberOfDrops));
            }
        }

        public void ValidateDedicatedNumberOfDrops(int dropsCount, int numberOfDrops)
        {
            if (dropsCount != numberOfDrops)
            {
                throw new UserFriendlyException(L("The number of drop points must be" + numberOfDrops));
            }
        }

        public void ValidateDedicatedNumberOfPickups(int PickupsCount, int numberOfPickups)
        {
            if (PickupsCount != numberOfPickups)
            {
                throw new UserFriendlyException(L("The number of pickup points must be" + numberOfPickups));
            }
        }

        public bool ValidateTripVasesNumber(long shippingRequestId,int tripVasNumber, long shippingRequestVasId)
        {
            var tripsNumber=_shippingRequestVasRepository.FirstOrDefault(x => x.Id == shippingRequestVasId && x.ShippingRequestId == shippingRequestId).NumberOfTrips;
            return tripVasNumber <= tripsNumber;
        }

        public Facility GetFacilityByPermission(string name,long shippingRequestId)
        {
            var request=GetShippingRequestByPermission(shippingRequestId);
            return _facilityRepository.GetAll()
                .WhereIf(_featureChecker.IsEnabled(AppFeatures.TachyonDealer),x=>x.TenantId==request.TenantId )
                .WhereIf(_featureChecker.IsEnabled(AppFeatures.Shipper), x => x.TenantId == _AbpSession.TenantId)
                .WhereIf(_featureChecker.IsEnabled(AppFeatures.CarrierAsASaas), x => x.TenantId == _AbpSession.TenantId)
                .FirstOrDefault(x => x.Name.Trim() == name.Trim());
        }

        public Receiver GetReceiverByPermissionAndFacility(string name, long shippingRequestId, long facilityId)
        {
            var request = GetShippingRequestByPermission(shippingRequestId);
            return _receiverRepository.GetAll()
                .Where(x=>x.FacilityId==facilityId)
                .WhereIf(_featureChecker.IsEnabled(AppFeatures.TachyonDealer), x => x.TenantId == request.TenantId)
                .WhereIf(_featureChecker.IsEnabled(AppFeatures.Shipper), x => x.TenantId == _AbpSession.TenantId)
                .WhereIf(_featureChecker.IsEnabled(AppFeatures.CarrierAsASaas), x => x.TenantId == _AbpSession.TenantId)
                .FirstOrDefault(x => x.FullName.Trim() == name.Trim());
        }


        public long GeDriverIdByPermission(string text, long shippingRequestId, int tenantId)
        {
            string firstname = text.Split(" ")[0];
            string surname = text.Split(" ")?[1];

            DisableTenancyFilters();
            return _dedicatedShippingRequestDriverRepository.GetAll().FirstOrDefault(x => x.DriverUser.TenantId == tenantId &&
            x.ShippingRequestId == shippingRequestId &&
            ((x.DriverUser.Name.ToLower() == firstname.ToLower() && x.DriverUser.Surname.ToLower() == surname.ToLower()) ||
            x.DriverUser.PhoneNumber == text)
            ).DriverUserId;
        }


        public long GetTruckIdByPermission(string text, long shippingRequestId, int tenantId)
        {
            DisableTenancyFilters();
            return _dedicatedShippingRequestTruckRepository.GetAll().FirstOrDefault(x => x.Truck.TenantId == tenantId && x.ShippingRequestId == shippingRequestId && x.Truck.PlateNumber == text).TruckId;
               
        }


        public ShippingRequestTrip GetShippingRequestTripIdByBulkRef(string tripReference, long shippingRequestId)
        {
           return _shippingRequestTripRepository.FirstOrDefault(x => x.BulkUploadRef == tripReference && x.ShippingRequestId==shippingRequestId);
        }

        public List<ShippingRequestTrip> GetShippingRequestTripsIdByBulkRefs(List<string> references)
        {
            return _shippingRequestTripRepository.GetAll().Where(x => references.Contains(x.BulkUploadRef)).ToList();
        }

        public ShippingRequest GetShippingRequestById(long id)
        {
            DisableTenancyFilters();
            return _shippingRequestRepository.GetAll()
                .WhereIf(_featureChecker.IsEnabled(AppFeatures.TachyonDealer), x => x.IsTachyonDeal)
                .WhereIf(!_featureChecker.IsEnabled(AppFeatures.TachyonDealer), x => x.TenantId == _AbpSession.TenantId)
                .FirstOrDefault(x => x.Id == id);
        }

        //goods details validation
        public void ValidateTotalweight(List<ICreateOrEditGoodsDetailDtoBase> input, ShippingRequest request)
        {
            if (request.TotalWeight > 0)
            {
                var totalWeight = input.Sum(g => g.Weight * g.Amount);
                if (totalWeight > request.TotalWeight)
                {
                    throw new UserFriendlyException(L(
                        "TheTotalWeightOfGoodsDetailsshouldNotBeGreaterThanShippingRequestWeight",
                        request.TotalWeight));
                }
            }
        }

        public void AssignWorkFlowVersionToRoutPoints(List<RoutPoint> routPoints, bool tripNeedsDeliveryNote, ShippingRequestTripFlag tripFlag)
        {
            if (routPoints == null || !routPoints.Any()) return;
            

            foreach (var point in routPoints)
            {
                  point.WorkFlowVersion = point.PickingType switch
                {
                    PickingType.Dropoff => GetDropOffWorkflowVersion(point),
                    PickingType.Pickup => tripFlag == ShippingRequestTripFlag.HomeDelivery ? WorkflowVersionConst.PickupHomeDeliveryWorkflowVersion : WorkflowVersionConst.PickupPointWorkflowVersion,
                    _ => throw new ArgumentOutOfRangeException(nameof(point.PickingType))
                };
                  
            }
            
            int GetDropOffWorkflowVersion(RoutPoint point)
            {
                switch (tripFlag)
                {
                    case ShippingRequestTripFlag.Normal:
                        return tripNeedsDeliveryNote
                            ? WorkflowVersionConst.DropOffWithDeliveryNotePointWorkflowVersion
                            : WorkflowVersionConst.DropOffWithoutDeliveryNotePointWorkflowVersion;
                    case ShippingRequestTripFlag.HomeDelivery:

                        switch (point.NeedsReceiverCode)
                        {
                            case true when point.NeedsPOD:
                                return WorkflowVersionConst.DropOffHomeDeliveryWithPodAndReceiverCodeWorkflowVersion;
                            case true when !point.NeedsPOD:
                                return WorkflowVersionConst.DropOffHomeDeliveryWithReceiverCodeWorkflowVersion;
                            case false when point.NeedsPOD:
                                return WorkflowVersionConst.DropOffHomeDeliveryWithPodWorkflowVersion;
                            case false when !point.NeedsPOD:
                                return WorkflowVersionConst.DropOffHomeDeliveryWorkflowVersion;
                        }

                        break;
                    default: throw new UserFriendlyException(L("TripTypeIsNotValid"));
                }

                throw new UserFriendlyException(L("TripTypeIsNotValid"));
            }
        }

        public async Task NotifyCarrierWithTripDetails(ShippingRequestTrip trip,
            int? carrierTenantId,
            bool hasAttachmentNotification,
            bool needseliverNoteNotification,
            bool hasAttachment)
        {
            //Notify carrier when trip has attachment or needs delivery note
            if (trip.ShippingRequestFk.CarrierTenantId != null && trip.HasAttachment && hasAttachmentNotification)
            {
                await _appNotifier.NotifyCarrierWhenTripHasAttachment(trip.Id, carrierTenantId, hasAttachment);
            }

            if (trip.ShippingRequestFk.CarrierTenantId != null && trip.NeedsDeliveryNote && needseliverNoteNotification)
            {
                await _appNotifier.NotifyCarrierWhenTripNeedsDeliverNote(trip.Id, carrierTenantId);
            }
        }

        public async Task ValidateTruckAndDriver(ICreateOrEditTripDtoBase input)
        {
            if (input.DriverUserId != null)
            {
                //Check if driver user is from assigned
                if (!await _dedicatedShippingRequestDriverRepository.GetAll().AnyAsync(x => x.ShippingRequestId == input.ShippingRequestId && x.DriverUserId == input.DriverUserId))
                {
                    throw new UserFriendlyException(L("DriverUserMustBeFromAssigned"));
                }
            }
            if (input.TruckId != null)
            {
                if (!await _dedicatedShippingRequestTruckRepository.GetAll().AnyAsync(x => x.ShippingRequestId == input.ShippingRequestId && x.TruckId == input.TruckId))
                {
                    throw new UserFriendlyException(L("TruckMustBeFromAssigned"));
                }
            }
        }

        public List<Facility> GetAllFacilitiesByIds(List<long> facilityIds)
        {
            DisableTenancyFilters();
            return _facilityRepository.GetAll().Where(x => facilityIds.Contains(x.Id)).ToList();
        }
        private void ValidateDuplicateBulkReferenceFromDB(ImportTripDto importTripDto, StringBuilder exceptionMessage)
        {
            var trip = _shippingRequestTripRepository.GetAll()
               .Where(x => x.ShippingRequestId == importTripDto.ShippingRequestId && x.BulkUploadRef == importTripDto.BulkUploadRef).FirstOrDefault();
            if (trip != null)
            {
                exceptionMessage.Append(L("TheBulkReferenceIsAlreadyExists")+";");
                importTripDto.Exception = exceptionMessage.ToString();
            }
        }
    }
}
