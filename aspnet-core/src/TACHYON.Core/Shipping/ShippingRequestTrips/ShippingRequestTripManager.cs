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
using TACHYON.Features;
using TACHYON.Goods.Dtos;
using TACHYON.Notifications;
using TACHYON.Receivers;
using TACHYON.Routs.RoutPoints;
using TACHYON.Routs.RoutPoints.Dtos;
using TACHYON.Shipping.ShippingRequests;
using TACHYON.Shipping.Trips.Dto;
using TACHYON.ShippingRequestVases;

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

        private IAbpSession _AbpSession { get; set; }


        public ShippingRequestTripManager(IRepository<ShippingRequestTrip> shippingRequestTripRepository, IRepository<ShippingRequest, long> shippingRequestRepository, IFeatureChecker featureChecker, IAbpSession abpSession, IRepository<RoutPoint, long> routePointRepository, IRepository<Facility, long> facilityRepository, IRepository<Receiver> receiverRepository, IAppNotifier appNotifier, IRepository<ShippingRequestVas, long> shippingRequestVasRepository)
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

            //StringBuilder exceptionMessage = new StringBuilder();
            if(importTripDto.StartTripDate?.Date == null)
            {
                importTripDto.StartTripDate = GetShippingRequestById(importTripDto.ShippingRequestId).StartTripDate;
            }
            if (importTripDto.EndTripDate != null && importTripDto.StartTripDate?.Date > importTripDto.EndTripDate.Value.Date)
            {
                exceptionMessage.Append("The start date must be or equal to end date." + "; ");
            }

            try
            {
                ValidateTripDates(importTripDto, SR);
            }
            catch (UserFriendlyException e)
            {
                exceptionMessage.Append(e.Message+";");
            }

            ValidateDuplicateBulkReferenceFromDB(importTripDto, exceptionMessage);
            //ValidateDuplicatedReference(importTripDtoList, exceptionMessage);
            //check if no duplication in reference from DB
            importTripDto.Exception = exceptionMessage.ToString();
        }

        public ShippingRequest GetShippingRequestByPermission(long id)
        {
            return _shippingRequestRepository.GetAll()
                .WhereIf(_featureChecker.IsEnabled(AppFeatures.TachyonDealer), x => x.IsTachyonDeal)
                .WhereIf(_featureChecker.IsEnabled(AppFeatures.Shipper), x => x.TenantId == _AbpSession.TenantId)
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
            return _shippingRequestRepository.GetAll()
                .WhereIf(_featureChecker.IsEnabled(AppFeatures.TachyonDealer), x => x.IsTachyonDeal)
                .WhereIf(_featureChecker.IsEnabled(AppFeatures.Shipper), x => x.TenantId == _AbpSession.TenantId)
                .WhereIf(_featureChecker.IsEnabled(AppFeatures.CarrierAsASaas), x => x.TenantId == _AbpSession.TenantId)
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

        public void AssignWorkFlowVersionToRoutPoints(List<RoutPoint> routPoints, bool tripNeedsDeliveryNote)
        {
            if (routPoints != null && routPoints.Any())
            {
                foreach (var point in routPoints)
                {
                    point.WorkFlowVersion = point.PickingType == PickingType.Pickup
                        ? TACHYONConsts.PickUpRoutPointWorkflowVersion
                        : tripNeedsDeliveryNote
                            ? TACHYONConsts.DropOfWithDeliveryNoteRoutPointWorkflowVersion
                            : TACHYONConsts.DropOfRoutPointWorkflowVersion;
                }
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
