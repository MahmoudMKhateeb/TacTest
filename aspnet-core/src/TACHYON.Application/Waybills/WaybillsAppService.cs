using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using TACHYON.DataExporting;
using TACHYON.Dto;
using TACHYON.Trucks;

namespace TACHYON.Waybills
{
    public class WaybillsAppService : TACHYONAppServiceBase
    {
        private readonly PdfExporterBase _pdfExporterBase;
        private readonly IRepository<Truck, Guid> _trucksRepository;

        public WaybillsAppService(PdfExporterBase pdfExporterBase, IRepository<Truck, Guid> trucksRepository)
        {
            _pdfExporterBase = pdfExporterBase;
            _trucksRepository = trucksRepository;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="PlateNumber"></param>
        /// <returns></returns>
        public FileDto GetSingleDropWaybillPdf(string PlateNumber)
        {
            var reportPath = "/Waybills/Reports/Single_Drop_Waybill.rdlc";
           
            var list = _trucksRepository.GetAll()
                .Select(x => new
                {
                    x.ModelName,
                    x.ModelYear,
                    x.PlateNumber,
                    x.CreatorUserId
                }).Where(t =>t.PlateNumber != PlateNumber && t.CreatorUserId !=null).ToList();

            ArrayList names = new ArrayList();
            ArrayList data = new ArrayList();

            names.Add("DataSet1");
            data.Add(list);
            
            return _pdfExporterBase.CreateRdlcPdfPackageFromList("Single_Drop_Waybill", reportPath, names, data);
        }


        public FileDto GetMultipleDropWaybillPdf()
        {
             var reportPath = "/Waybills/Reports/Multiple_Drop_Waybill.rdlc";

            var list = _trucksRepository.GetAll()
                .Select(x => new
                {
                    x.ModelName,
                    x.ModelYear,
                    x.PlateNumber
                }).ToList();

            ArrayList names = new ArrayList();
            ArrayList data = new ArrayList();

            names.Add("DataSet1");
            data.Add(list);

            return _pdfExporterBase.CreateRdlcPdfPackageFromList("Multiple_Drop_Waybill", reportPath, names, data);
        }


        public FileDto GetMasterWaybillPdf()
        {       
            var reportPath = "/Waybills/Reports/Master_Waybill.rdlc";

            var list = _trucksRepository.GetAll()
                .Select(x => new
                {
                    x.ModelName,
                    x.ModelYear,
                    x.PlateNumber
                }).ToList();

            ArrayList names = new ArrayList();
            ArrayList data = new ArrayList();

            names.Add("DataSet1");
            data.Add(list);

            return _pdfExporterBase.CreateRdlcPdfPackageFromList("Master_Waybill", reportPath, names, data);
        }

    }
}
