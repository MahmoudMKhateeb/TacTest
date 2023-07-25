using Abp.Application.Features;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Web.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
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
    [RequiresFeature(AppFeatures.FinancialReportFeature)]
    public class FinancialReportDataSourceAppService : TACHYONAppServiceBase
    {
        private readonly IReportParameterManager _parameterManager;
        private readonly IRepository<ShippingRequestTrip> _tripRepository;
        private readonly IRepository<InvoiceTrip, long> _invoiceTripRepository;

        private const string EmptyField = "__";

        public FinancialReportDataSourceAppService(
            IReportParameterManager parameterManager,
            IRepository<ShippingRequestTrip> tripRepository,
            IRepository<InvoiceTrip, long> invoiceTripRepository)
        {
            _parameterManager = parameterManager;
            _tripRepository = tripRepository;
            _invoiceTripRepository = invoiceTripRepository;
        }


        public async Task<IEnumerable<FinancialItem>> GetAll(Guid reportId)
        {
            var reportParameters = await _parameterManager.GetReportParameters(reportId);

            DisableTenancyFilters();

            var tripsQuery = _tripRepository.GetAll().AsNoTracking()
                .Where(x => x.ShippingRequestId.HasValue
                    ? (x.ShippingRequestFk.TenantId == AbpSession.TenantId ||
                       x.ShippingRequestFk.CarrierTenantId == AbpSession.TenantId)
                    : (x.CarrierTenantId == AbpSession.TenantId || x.ShipperTenantId == AbpSession.TenantId))
                .ApplyReportParameters(reportParameters);

            var financialItems = await (from trip in tripsQuery
                    from carrierInvoice in _invoiceTripRepository.GetAll().Where(i => trip.Id == i.TripId
                        && ((trip.ShippingRequestId.HasValue
                            ? trip.ShippingRequestFk.CarrierTenantId
                            : trip.CarrierTenantId) == i.InvoiceFK.TenantId)).DefaultIfEmpty()
                    from shipperInvoice in _invoiceTripRepository.GetAll().Where(i => trip.Id == i.TripId
                        && ((trip.ShippingRequestId.HasValue
                                ? trip.ShippingRequestFk.TenantId
                                : trip.ShipperTenantId) ==
                            i.InvoiceFK.TenantId)).DefaultIfEmpty()
                    select new
                    {
                        Trip = trip,
                        CarrierInvoice = carrierInvoice,
                        ShipperInvoice = shipperInvoice,
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
                .Select(x => new FinancialItem
                {
                    Destination = x.Trip.DestinationFacilityFk.CityFk.DisplayName,
                    Origin = x.Trip.OriginFacilityFk.CityFk.DisplayName,
                    TripOffloadingDate =
                        x.LastCompletedPointFinishOffloadingDate != null
                            ? x.LastCompletedPointFinishOffloadingDate.ToString("dd/MM/yyyy")
                            : EmptyField,
                    TripLoadingDate = x.LoadingDate != null ? x.LoadingDate.ToString("dd/MM/yyyy") : EmptyField,
                    WaybillNumber = x.Trip.WaybillNumber.ToString(),
                    ShipperName =
                        x.Trip.ShippingRequestId.HasValue
                            ? x.Trip.ShippingRequestFk.Tenant.Name
                            : x.Trip.ShipperTenantFk.Name,
                    CarrierName = x.Trip.ShippingRequestId.HasValue
                        ? (x.Trip.ShippingRequestFk.CarrierTenantId.HasValue
                            ? x.Trip.ShippingRequestFk.CarrierTenantFk.Name
                            : EmptyField)
                        : (x.Trip.CarrierTenantId.HasValue ? x.Trip.CarrierTenantFk.Name : EmptyField),
                    RequestId =
                        x.Trip.ShippingRequestId.HasValue ? x.Trip.ShippingRequestFk.ReferenceNumber : EmptyField,
                    TransportType =
                        x.Trip.ShippingRequestId.HasValue
                            ? x.Trip.ShippingRequestFk.TransportTypeFk.Translations
                                .FirstOrDefault(t => t.Language.Contains(CultureInfo.CurrentUICulture.Name))
                                .TranslatedDisplayName
                            : x.Trip.AssignedTruckFk.TransportTypeFk.Translations
                                .FirstOrDefault(t => t.Language.Contains(CultureInfo.CurrentUICulture.Name))
                                .TranslatedDisplayName,
                    TruckType =
                        x.Trip.ShippingRequestId.HasValue
                            ? x.Trip.ShippingRequestFk.TrucksTypeFk.Translations
                                .FirstOrDefault(t => t.Language.Contains(CultureInfo.CurrentUICulture.Name))
                                .TranslatedDisplayName
                            : x.Trip.AssignedTruckFk.TrucksTypeFk.Translations
                                .FirstOrDefault(t => t.Language.Contains(CultureInfo.CurrentUICulture.Name))
                                .TranslatedDisplayName,
                    TripStatus = LocalizationSource.GetString(x.Trip.Status.ToString()),
                    TripCreationDate = x.Trip.CreationTime.ToString("dd/MM/yyyy"),
                    PodStatus =
                        x.Trip.RoutPoints.Where(p => p.PickingType == PickingType.Dropoff).All(p => p.IsPodUploaded)
                            ? LocalizationSource.GetString("Submitted")
                            : LocalizationSource.GetString("NotSubmitted"),
                    RequestCreationDate =
                        x.Trip.ShippingRequestId.HasValue
                            ? x.Trip.ShippingRequestFk.CreationTime.ToString("dd/MM/yyyy")
                            : EmptyField,
                    CarrierInvoiceNumber = x.CarrierInvoice != null ? x.CarrierInvoice.InvoiceFK.InvoiceNumber : null,
                    CarrierInvoiceStatus =
                        x.CarrierInvoice != null
                            ? LocalizationSource.GetString(x.CarrierInvoice.InvoiceFK.Status.ToString())
                            : EmptyField,
                    ShipperInvoiceNumber = x.ShipperInvoice != null ? x.ShipperInvoice.InvoiceFK.InvoiceNumber : null,
                    ShipperInvoiceStatus =
                        x.ShipperInvoice != null
                            ? LocalizationSource.GetString(x.ShipperInvoice.InvoiceFK.Status.ToString())
                            : EmptyField,
                    CostWithoutVat = x.Trip.SubTotalAmount,
                    CostWithVat = x.Trip.TotalAmount,
                    SellingWithoutVat = x.Trip.SubTotalAmountWithCommission,
                    SellingWithVat = x.Trip.TotalAmountWithCommission,
                    TachyonCommissionWithVat = x.Trip.VatAmountWithCommission,
                    TachyonCommissionWithoutVat = x.Trip.CommissionAmount,
                    ShipperInvoiceConfirmationDate =
                        x.ShipperInvoice != null && x.ShipperInvoice.InvoiceFK.ConfirmationDate.HasValue
                            ? x.ShipperInvoice.InvoiceFK.ConfirmationDate.Value.ToString("dd/MM/yyyy")
                            : EmptyField,
                    ShipperInvoiceIssuanceDate = x.ShipperInvoice != null
                        ? x.ShipperInvoice.InvoiceFK.CreationTime.ToString("dd/MM/yyyy")
                        : EmptyField,
                }).ToListAsync();

            return financialItems;
        }
    }
}