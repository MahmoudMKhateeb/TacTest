using Abp.Application.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using TACHYON.Dashboards.Host.Dto;
using TACHYON.Dashboards.Shipper.Dto;

namespace TACHYON.Dashboards.Shipper
{
    public interface IShipperDashboardAppService : IApplicationService
    {
        Task<List<ListPerMonthDto>> GetCompletedTripsCountPerMonth(GetDataByDateFilterInput input);
        Task<AcceptedAndRejectedRequestsListDto> GetAcceptedAndRejectedRequests(GetDataByDateFilterInput input);
        Task<List<MostCarriersWorksListDto>> GetMostWorkedWithCarriers();
        Task<CompletedTripVsPodListDto> GetCompletedTripVsPod(GetDataByDateFilterInput input);
        Task<InvoicesVsPaidInvoicesDto> GetInvoicesVSPaidInvoices(GetDataByDateFilterInput input);
        Task<List<RequestsInMarketpalceDto>> GetRequestsInMarketpalce(GetDataByDateFilterInput input);
        Task<List<MostUsedOriginsDto>> GetMostUsedOrigins();
        Task<List<MostUsedOriginsDto>> GetMostUsedDestinatiions();
        Task<long> GetDocumentsDueDateInDays();
        Task<long> GetInvoiceDueDateInDays();
        Task<List<TrackingMapDto>> GetTrackingMap();
    }
}