using Abp.Application.Features;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Web.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TACHYON.Features;
using TACHYON.Invoices;
using TACHYON.Reports.ExtensionMethods;
using TACHYON.Reports.ReportModels;
using TACHYON.Reports.ReportParameters;
using TACHYON.Routs.RoutPoints;
using TACHYON.Shipping.ShippingRequestTrips;

namespace TACHYON.Reports.ReportDataSources
{

    [DontWrapResult(WrapOnSuccess = false, WrapOnError = true)]
    [AbpAuthorize]
    [RequiresFeature(AppFeatures.PodPerformanceReport)]
    public class PodPerformanceReportDataSourceAppService : TACHYONAppServiceBase
    {
        private readonly IRepository<ShippingRequestTrip> _tripRepository;
        private readonly IReportParameterManager _parameterManager;
        private readonly IRepository<InvoiceTrip, long> _invoiceTripRepository;

        private const string EmptyField = "__";

        public PodPerformanceReportDataSourceAppService(
            IRepository<ShippingRequestTrip> tripRepository,
            IReportParameterManager parameterManager,
            IRepository<InvoiceTrip, long> invoiceTripRepository)
        {
            _tripRepository = tripRepository;
            _parameterManager = parameterManager;
            _invoiceTripRepository = invoiceTripRepository;
        }

        public async Task<IEnumerable<PodPerformanceItem>> GetAll(Guid reportId)
        {
            var parameterList = await _parameterManager.GetReportParameters(reportId);
            bool isCarrier = AbpSession.TenantId.HasValue && await IsCarrier();
            bool isShipper = AbpSession.TenantId.HasValue && await IsShipper();
            DisableTenancyFilters();

            var tripsQuery = _tripRepository.GetAll().AsNoTracking()
                .Where(x => x.ShippingRequestId.HasValue
                    ? (x.ShippingRequestFk.TenantId == AbpSession.TenantId ||
                       x.ShippingRequestFk.CarrierTenantId == AbpSession.TenantId)
                    : (x.CarrierTenantId == AbpSession.TenantId || x.ShipperTenantId == AbpSession.TenantId))
                .ApplyReportParameters(parameterList);

            var podReportItems = await (from trip in tripsQuery
                    from invoiceTrip in _invoiceTripRepository.GetAll().Where(i => trip.Id == i.TripId).DefaultIfEmpty()
                    select new
                    {
                        Trip = trip,
                        Invoice = invoiceTrip.InvoiceFK,
                        LastUploadedPodDate = trip.RoutPoints.Where(p => p.PickingType == PickingType.Dropoff)
                            .SelectMany(p => p.RoutPointStatusTransitions).Where(p =>
                                !p.IsReset && p.Status == RoutePointStatus.DeliveryConfirmation)
                            .Select(p => p.CreationTime).OrderByDescending(x => x).First(),
                        LastCompletedPointFinishOffloadingDate = trip.RoutPoints
                            .Where(p => p.PickingType == PickingType.Dropoff)
                            .SelectMany(p => p.RoutPointStatusTransitions).Where(p =>
                                !p.IsReset && p.Status == RoutePointStatus.FinishOffLoadShipment)
                            .Select(p => p.CreationTime).OrderByDescending(x => x).FirstOrDefault(),
                        LoadingDate = trip.RoutPoints.Where(x => x.PickingType == PickingType.Pickup)
                            .SelectMany(x => x.RoutPointStatusTransitions)
                            .Where(x => x.Status == RoutePointStatus.StartLoading)
                            .Select(x => x.CreationTime).FirstOrDefault()
                    })
                .Select(x => new PodPerformanceItem
                {
                    AccountManager = x.Trip.ShippingRequestId.HasValue
                        ? (isShipper
                            ? x.Trip.ShippingRequestFk.Tenant.FinancialName
                            : (isCarrier ? x.Trip.ShippingRequestFk.CarrierTenantFk.FinancialName : EmptyField))
                        : (isShipper
                            ? x.Trip.ShipperTenantFk.FinancialName
                            : (isCarrier ? x.Trip.CarrierTenantFk.FinancialName : EmptyField)),
                    BookingNumber =
                        x.Trip.ShippingRequestId.HasValue ? x.Trip.ShippingRequestFk.ShipperInvoiceNo : EmptyField,
                    ContainerNumber =
                        string.IsNullOrEmpty(x.Trip.ContainerNumber) ? x.Trip.ContainerNumber : EmptyField,
                    CarrierName = x.Trip.ShippingRequestId.HasValue
                        ? (x.Trip.ShippingRequestFk.CarrierTenantId.HasValue
                            ? x.Trip.ShippingRequestFk.CarrierTenantFk.Name
                            : EmptyField)
                        : (x.Trip.CarrierTenantId.HasValue ? x.Trip.CarrierTenantFk.Name : EmptyField),
                    Destination = x.Trip.DestinationFacilityFk.CityFk.DisplayName,
                    ShipperName =
                        x.Trip.ShippingRequestId.HasValue
                            ? x.Trip.ShippingRequestFk.Tenant.Name
                            : x.Trip.ShipperTenantFk.Name,
                    DriverName =
                        x.Trip.AssignedDriverUserId.HasValue
                            ? $"{x.Trip.AssignedDriverUserFk.Name} {x.Trip.AssignedDriverUserFk.Surname}"
                            : EmptyField,
                    InvoiceStatus = isShipper
                        ? LocalizationSource.GetString((x.Trip.IsShipperHaveInvoice
                            ? "InvoiceIssued"
                            : "InvoiceNotIssued"))
                        : (isCarrier
                            ? LocalizationSource.GetString((x.Trip.IsCarrierHaveInvoice
                                ? "InvoiceIssued"
                                : "InvoiceNotIssued"))
                            : EmptyField),
                    InvoiceNumber = x.Invoice.InvoiceNumber.HasValue ? x.Invoice.InvoiceNumber.ToString() : EmptyField,
                    WaybillNumber = x.Trip.WaybillNumber,
                    ShippingType = x.Trip.ShippingRequestId.HasValue
                        ? LocalizationSource.GetString(x.Trip.ShippingRequestFk.ShippingTypeId.ToString())
                        : (x.Trip.ShippingTypeId.HasValue
                            ? LocalizationSource.GetString(x.Trip.ShippingTypeId.ToString())
                            : EmptyField),
                    RequestReferenceNumber =
                        x.Trip.ShippingRequestId.HasValue ? x.Trip.ShippingRequestFk.ReferenceNumber : EmptyField,
                    ShipperNumber = x.Trip.ShippingRequestFk.ShipperReference,
                    Origin = x.Trip.OriginFacilityFk.CityFk.DisplayName,
                    OffloadingDate =
                        x.LastCompletedPointFinishOffloadingDate != null
                            ? x.LastCompletedPointFinishOffloadingDate.ToString("dd/MM/yyyy")
                            : EmptyField,
                    LoadingDate = x.LoadingDate != null ? x.LoadingDate.ToString("dd/MM/yyyy") : EmptyField,
                    PodStatus = x.Trip.RoutPoints.Where(p => p.PickingType == PickingType.Dropoff)
                        .All(p => p.IsPodUploaded)
                        ? LocalizationSource.GetString("Submitted")
                        : LocalizationSource.GetString("NotSubmitted"),
                    PlateNumber = x.Trip.AssignedTruckId.HasValue ? x.Trip.AssignedTruckFk.PlateNumber : EmptyField,
                    IsUploadedWithin7Days = x.LastCompletedPointFinishOffloadingDate.AddDays(7) > x.LastUploadedPodDate
                        ? LocalizationSource.GetString("Yes")
                        : LocalizationSource.GetString("No"),
                }).ToListAsync();

            return podReportItems;
        }
    }
}