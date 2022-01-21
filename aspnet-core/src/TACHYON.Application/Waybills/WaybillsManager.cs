using Abp.Domain.Repositories;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TACHYON.DataExporting;
using TACHYON.Goods.GoodsDetails;
using TACHYON.Routs.RoutPoints;
using TACHYON.Shipping.ShippingRequests;
using TACHYON.Shipping.ShippingRequestTrips;

namespace TACHYON.Waybills
{
    public class WaybillsManager : TACHYONDomainServiceBase
    {
        private readonly ShippingRequestsAppService _shippingRequestAppService;
        private readonly RoutPointsAppService _routPointAppService;
        private readonly PdfExporterBase _pdfExporterBase;
        private readonly GoodsDetailsAppService _goodsDetailsAppService;
        private readonly IRepository<ShippingRequestTrip> _shippingRequestTripRepository;

        public WaybillsManager(ShippingRequestsAppService shippingRequestAppService,
            RoutPointsAppService routPointAppService,
            PdfExporterBase pdfExporterBase,
            GoodsDetailsAppService goodsDetailsAppService,
            IRepository<ShippingRequestTrip> shippingRequestTripRepository)
        {
            _shippingRequestAppService = shippingRequestAppService;
            _routPointAppService = routPointAppService;
            _pdfExporterBase = pdfExporterBase;
            _goodsDetailsAppService = goodsDetailsAppService;
            _shippingRequestTripRepository = shippingRequestTripRepository;
        }

        public byte[] GetSingleDropOrMasterWaybillPdf(int shippingRequestTripId)
        {
            DisableTenancyFilters();
            if (IsSingleDropShippingRequest(shippingRequestTripId))
            {
                return GetSingleDropWaybillPdf(shippingRequestTripId);
            }
            else
            {
                return GetMasterWaybillPdf(shippingRequestTripId);
            }
        }

        public byte[] GetMultipleDropWaybillPdf(long id)
        {
            DisableTenancyFilters();
            if (IsSingleDropShippingRequest(id))
            {
                throw new UserFriendlyException(L("Cannot download drop waybill for single drop shipping request"));
            }

            var reportPath = "/Waybills/Reports/Multiple_Drop_Waybill.rdlc";

            ArrayList names = new ArrayList();
            ArrayList data = new ArrayList();

            names.Add("MultipleDropDataSet");
            data.Add(_shippingRequestAppService.GetMultipleDropWaybill(id));

            names.Add("MultipleDropsGoodsDetailsDataSet");
            data.Add(_goodsDetailsAppService.GetShippingrequestGoodsDetailsForMultipleDropWaybill(id));

            names.Add("MultipleDropsVasDataSet");
            data.Add(_shippingRequestAppService.GetShippingRequestVasesForMultipleDropWaybill(id));

            return _pdfExporterBase.GetRdlcPdfPackageAsBinaryData(reportPath, names, data);
        }

        private byte[] GetMasterWaybillPdf(int shippingRequestTripId)
        {
            DisableTenancyFilters();
            var reportPath = "/Waybills/Reports/Master_Waybill.rdlc";

            ArrayList names = new ArrayList();
            ArrayList data = new ArrayList();

            names.Add("DataSet1");
            data.Add(_shippingRequestAppService.GetMasterWaybill(shippingRequestTripId));

            names.Add("GetDropDetailsDS");
            data.Add(_routPointAppService.GetDropsDetailsForMasterWaybill(shippingRequestTripId));

            names.Add("SingleDropVasDataSet");
            data.Add(_shippingRequestAppService.GetShippingRequestVasesForSingleDropWaybill(shippingRequestTripId));
            return _pdfExporterBase.GetRdlcPdfPackageAsBinaryData(reportPath, names, data);
        }

        private byte[] GetSingleDropWaybillPdf(int shippingRequestTripId)
        {
            DisableTenancyFilters();
            var reportPath = "/Waybills/Reports/Single_Drop_Waybill.rdlc";

            ArrayList names = new ArrayList();
            ArrayList data = new ArrayList();

            names.Add("SingleDropDataSet");
            data.Add(_shippingRequestAppService.GetSingleDropWaybill(shippingRequestTripId));

            names.Add("SingleDropGoodsDetailsDataSet");
            data.Add(_goodsDetailsAppService.GetShippingrequestGoodsDetailsForSingleDropWaybill(shippingRequestTripId));

            names.Add("SingleDropVasDataSet");
            data.Add(_shippingRequestAppService.GetShippingRequestVasesForSingleDropWaybill(shippingRequestTripId));

            return _pdfExporterBase.GetRdlcPdfPackageAsBinaryData(reportPath, names, data);
        }


        private bool IsSingleDropShippingRequest(int shippingRequestTripId)
        {
            var item = _shippingRequestTripRepository
                .GetAll()
                .Include(e => e.ShippingRequestFk)
                .FirstOrDefault(e => e.Id == shippingRequestTripId);
            return item.ShippingRequestFk.RouteTypeId == ShippingRequestRouteType.SingleDrop;
        }

        private bool IsSingleDropShippingRequest(long routPointId)
        {
            var item = _shippingRequestTripRepository
                .GetAll()
                .Include(e => e.ShippingRequestFk)
                .Include(e => e.RoutPoints)
                .FirstOrDefault(e => e.RoutPoints.Any(x => x.Id == routPointId));

            return item.ShippingRequestFk.RouteTypeId == ShippingRequestRouteType.SingleDrop;
        }
    }
}