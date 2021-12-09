using Abp.Application.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using TACHYON.Dashboards.Host.Dto;
using TACHYON.Dashboards.Shipper.Dto;

namespace TACHYON.Dashboards.Shipper
{
    public interface IShipperDashboardAppService : IApplicationService
    {
        Task<List<ListPerMonthDto>> GetCompletedTripsCountPerMonth();
        Task<AcceptedAndRejectedRequestsListDto> GetAcceptedAndRejectedRequests();
        Task<List<MostCarriersWorksListDto>> GetMostWorkedWithCarriers();
        Task<CompletedTripVsPodListDto> GetCompletedTripVsPod();
        Task<InvoicesVsPaidInvoicesDto> GetInvoicesVSPaidInvoices();
        Task<List<RequestsInMarketpalceDto>> GetRequestsInMarketpalce();
        Task<List<MostUsedOriginsDto>> GetMostUsedOrigins();
        Task<List<MostUsedOriginsDto>> GetMostUsedDestinatiions();
        Task<long> GetDocumentsDueDateInDays();
        Task<long> GetInvoiceDueDateInDays();
    }
}