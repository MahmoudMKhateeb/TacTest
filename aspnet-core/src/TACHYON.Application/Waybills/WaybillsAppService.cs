using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using TACHYON.DataExporting;
using TACHYON.Dto;
using TACHYON.Goods.GoodsDetails;
using TACHYON.Shipping.ShippingRequests;
using TACHYON.Trucks;

namespace TACHYON.Waybills
{
    public class WaybillsAppService : TACHYONAppServiceBase
    {
        private readonly PdfExporterBase _pdfExporterBase;
        private readonly IRepository<Truck, long> _trucksRepository;
        private readonly ShippingRequestsAppService _shippingRequestAppService;
        private readonly GoodsDetailsAppService _goodsDetailsAppService;

        public WaybillsAppService(PdfExporterBase pdfExporterBase,
            IRepository<Truck, long> trucksRepository,
            ShippingRequestsAppService shippingRequestAppService,
                GoodsDetailsAppService goodsDetailsAppService)
        {
            _pdfExporterBase = pdfExporterBase;
            _trucksRepository = trucksRepository;
            _shippingRequestAppService = shippingRequestAppService;
            _goodsDetailsAppService = goodsDetailsAppService;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="PlateNumber"></param>
        /// <returns></returns>
        public FileDto GetSingleDropWaybillPdf(string platenumber)
        {
            var reportPath = "/Waybills/Reports/Single_Drop_Waybill.rdlc";
            
            ArrayList names = new ArrayList();
            ArrayList data = new ArrayList();

            names.Add("SingleDropDataSet");
            data.Add(_shippingRequestAppService.GetSingleDropWaybill());

            //names.Add("SingleDropGoodsDetailsDataSet");
            //data.Add(_goodsDetailsAppService.GetShippingrequestGoodsDetailsForSingleDropWaybill());

            //names.Add("SingleDropVasDataSet");
            //data.Add(_shippingRequestAppService.GetShippingRequestVasesForSingleDropWaybill(""shippingRequestId""));

            return _pdfExporterBase.CreateRdlcPdfPackageFromList("Single_Drop_Waybill", reportPath, names, data);
        }


        public FileDto GetMultipleDropWaybillPdf()
        {
             var reportPath = "/Waybills/Reports/Multiple_Drop_Waybill.rdlc";

             ArrayList names = new ArrayList();
             ArrayList data = new ArrayList();

             names.Add("MultipleDropDataSet");
             data.Add(_shippingRequestAppService.GetMultipleDropWaybill());

             //names.Add("MultipleDropsGoodsDetailsDataSet");
             //data.Add(_goodsDetailsAppService.GetShippingrequestGoodsDetailsForMultipleDropWaybill());

             names.Add("MultipleDropsVasDataSet");
             data.Add(_shippingRequestAppService.GetShippingRequestVasesForMultipleDropWaybill());


            return _pdfExporterBase.CreateRdlcPdfPackageFromList("Multiple_Drop_Waybill", reportPath, names, data);
        }


        public FileDto GetMasterWaybillPdf()
        {       
            var reportPath = "/Waybills/Reports/Master_Waybill.rdlc";
            
            ArrayList names = new ArrayList();
            ArrayList data = new ArrayList();

            names.Add("DataSet1");
            data.Add(_shippingRequestAppService.GetMasterWaybill(80));

            names.Add("SingleDropVasDataSet");
            data.Add(_shippingRequestAppService.GetShippingRequestVasesForSingleDropWaybill(80));
            return _pdfExporterBase.CreateRdlcPdfPackageFromList("Master_Waybill", reportPath, names, data);
        }

    }
}
